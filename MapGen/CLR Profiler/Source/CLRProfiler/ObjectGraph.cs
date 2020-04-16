// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections;
using System.Text;
using System.Diagnostics;

namespace CLRProfiler
{
    /// <summary>
    /// Summary description for ObjectGraph.
    /// </summary>
    public class ObjectGraph
    {
        internal ObjectGraph(ReadNewLog readNewLog, int tickIndex)
        {
            //
            // TODO: Add constructor logic here
            //
            idToObject = new Hashtable();
            typeNameToGcType = new Hashtable();
            unknownType = GetOrCreateGcType("<unknown type>", 0);   
            this.readNewLog = readNewLog;
            this.tickIndex = tickIndex;
        }

        internal class GcObject
        {
            internal GcObject(uint id)
            {
                this.id = id;
            }

            internal GcObject(uint id, uint size, GcObject refernces)
            {
                this.id = id;
                this.size = size;
                this.references = references;
            }

            internal uint size;          // Size of this object
            internal uint id;            // The object's id (i.e. really its address) -- not needed for now
            internal GcType type;
            internal int level;
            internal GcObject next;
            internal GcObject parent;
            internal Vertex vertex;
            internal GcObject[] references;
            internal InterestLevel interestLevel;
            internal int typeSizeStackTraceId;
            internal int allocTickIndex;
        }

        internal class GcType : IComparable
        {
            internal GcType(string name, int typeID)
            {
                this.name = name;
                this.typeID = typeID;
            }
            internal string name;
            internal uint cumulativeSize;            // Size for all the instances of that type

            internal int index;
            internal int typeID;

            internal InterestLevel interestLevel;

            public int CompareTo(Object o)
            {
                GcType t = (GcType)o;
                if (t.cumulativeSize < this.cumulativeSize)
                    return -1;
                else if (t.cumulativeSize > this.cumulativeSize)
                    return 1;
                else
                    return 0;
            }

        }

        internal Hashtable  idToObject;
        internal Hashtable  typeNameToGcType;
        internal GcType unknownType;
        internal ReadNewLog readNewLog;
        internal int tickIndex;

        internal GcObject[] roots;

        internal GcObject GetOrCreateObject(uint objectID)
        {
            GcObject o = (GcObject)idToObject[objectID];
            if (o == null)
            {
                o = new GcObject(objectID);
                idToObject[objectID] = o;
                o.type = unknownType;

            }
            return o;
        }

        internal GcType GetOrCreateGcType(string typeName, int typeID)
        {
            Debug.Assert(typeName != null);
            GcType t = (GcType)typeNameToGcType[typeName];
            if (t == null)
            {
                t = new GcType(typeName, typeID);
                typeNameToGcType[typeName] = t;
            }
            return t;
        }

        internal GcObject[] LookupReferences(int count, uint[] refIDs)
        {
            if (count == 0)
                return null;
            GcObject[] result = new GcObject[count];
            for (int i = 0; i < count; i++)
                result[i] = GetOrCreateObject(refIDs[i]);
            return result;
        }

        internal void ObjectGraphStatistics()
        {
            ArrayList gcTypes = new ArrayList();
            foreach (GcType gcType in typeNameToGcType.Values)
            {
                gcTypes.Add(gcType);
            }

            gcTypes.Sort();

            uint totalSize = 0;
            foreach (GcType gcType in gcTypes)
            {
                totalSize += gcType.cumulativeSize;
            }
        }

        void AssignLevels(GcObject rootObject)
        {
            // We use a breadth first traversal of the object graph.
            // To do this, we make use of a queue of objects still to process.
            GcObject head, tail;

            // Initialize
            head = rootObject;
            tail = rootObject;
            rootObject.level = 0;

            // Loop
            while (head != null)
            {
                if (head.references != null)
                {
                    int nextLevel = head.level + 1;
                    foreach (GcObject refObject in head.references)
                    {
                        if (refObject.level > nextLevel)
                        {
                            refObject.parent = head;
                            refObject.level = nextLevel;
                            tail.next = refObject;
                            tail = refObject;
                        }
                    }
                }
                head = head.next;
            }
        }

        int[] typeHintTable;

        internal Vertex FindVertex(GcObject gcObject, Graph graph, BuildTypeGraphOptions options)
        {
            if (gcObject.vertex != null)
                return gcObject.vertex;
            string signature = null;
            StringBuilder sb = new StringBuilder();
            if (gcObject.parent != null)
            {
                switch (options)
                {
                    case    BuildTypeGraphOptions.IndividualObjects:
                        sb.AppendFormat("Address = 0x{0:x}, size = {1:n0} bytes", gcObject.id, gcObject.size);
                        break;

                    case    BuildTypeGraphOptions.LumpBySignature:
                        sb.Append(gcObject.parent.type.name);
                        sb.Append("->");
                        sb.Append(gcObject.type.name);
                        if (gcObject.references != null)
                        {
                            sb.Append("->(");

                            ArrayList al = new ArrayList();
                            string separator = "";
                            const int MAXREFTYPECOUNT = 3;
                            int refTypeCount = 0;
                            for (int i = 0; i < gcObject.references.Length; i++)
                            {
                                GcObject refObject = gcObject.references[i];
                                GcType refType = refObject.type;
                                if (typeHintTable[refType.index] < i && gcObject.references[typeHintTable[refType.index]].type == refType)
                                {
                                    ;   // we already found this type - ignore further occurrences
                                }
                                else
                                {
                                    typeHintTable[refType.index] = i;
                                    refTypeCount++;
                                    if (refTypeCount <= MAXREFTYPECOUNT)
                                    {
                                        al.Add(refType.name);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            al.Sort();
                            foreach (string typeName in al)
                            {
                                sb.Append(separator);
                                separator = ",";
                                sb.Append(typeName);
                            }

                            if (refTypeCount > MAXREFTYPECOUNT)
                                sb.Append(",...");

                            sb.Append(")");
                        }
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }
                signature = sb.ToString();
            }

            gcObject.vertex = graph.FindOrCreateVertex(gcObject.type.name, signature, null);

            return gcObject.vertex;
        }

        internal static Graph graph;
        private static int graphFilterVersion;

        const int historyDepth = 3;

        // Check whether we have a parent object interested in this one
        private bool CheckForParentMarkingDescendant(GcObject gcObject)
        {
            GcObject parentObject = gcObject.parent;
            if (parentObject == null)
                return false;
            switch (parentObject.interestLevel & InterestLevel.InterestingChildren)
            {
                // Parent says it wants to show children
                case    InterestLevel.InterestingChildren:
                    gcObject.interestLevel |= InterestLevel.Display;
                    return true;

                // Parent is not interesting - check its parent
                case    InterestLevel.Ignore:
                    if (CheckForParentMarkingDescendant(parentObject))
                    {
                        gcObject.interestLevel |= InterestLevel.Display;
                        return true;
                    }
                    else
                        return false;

                default:
                    return false;
            }
        }

        private void AssignInterestLevels()
        {
            foreach (GcType gcType in typeNameToGcType.Values)
            {
                // Otherwise figure which the interesting types are.
                gcType.interestLevel = FilterForm.InterestLevelOfTypeName(gcType.name, readNewLog.finalizableTypes[gcType.typeID] != null);
            }

            foreach (GcObject gcObject in idToObject.Values)
            {
                // The initial interest level in objects is the one of their type
                gcObject.interestLevel = gcObject.type.interestLevel;
            }

            foreach (GcObject gcObject in idToObject.Values)
            {
                // Check if this is an interesting object, and we are supposed to mark its ancestors
                if ((gcObject.interestLevel & InterestLevel.InterestingParents) == InterestLevel.InterestingParents)
                {
                    for (GcObject parentObject = gcObject.parent; parentObject != null; parentObject = parentObject.parent)
                    {
                        // As long as we find uninteresting object, mark them for display
                        // When we find an interesting object, we stop, because either it
                        // will itself mark its parents, or it isn't interested in them (and we
                        // respect that despite the interest of the current object, somewhat arbitrarily).
                        if ((parentObject.interestLevel & InterestLevel.InterestingParents) == InterestLevel.Ignore)
                            parentObject.interestLevel |= InterestLevel.Display;
                        else
                            break;
                    }
                }
                // Check if this object should be displayed because one of its ancestors
                // is interesting, and it says its descendents are interesting as well
                if ((gcObject.interestLevel & (InterestLevel.Interesting|InterestLevel.Display)) == InterestLevel.Ignore)
                {
                    CheckForParentMarkingDescendant(gcObject);
                }
            }
        }

        internal enum BuildTypeGraphOptions
        {
            LumpBySignature,
            IndividualObjects
        }

        internal Graph BuildTypeGraph()
        {
            return BuildTypeGraph(-1, int.MaxValue, BuildTypeGraphOptions.LumpBySignature);
        }

        internal Graph BuildTypeGraph(int allocatedAfterTickIndex, int allocatedBeforeTickIndex, BuildTypeGraphOptions options)
        {
            if (graph == null || FilterForm.filterVersion != graphFilterVersion || allocatedAfterTickIndex >= 0 || allocatedBeforeTickIndex < int.MaxValue)
            {
                graph = new Graph(this);
                graph.graphType = Graph.GraphType.HeapGraph;
                graphFilterVersion = FilterForm.filterVersion;
                graph.previousGraphTickIndex = allocatedAfterTickIndex;
            }
            else
            {
                ObjectGraph previousGraph = (ObjectGraph)graph.graphSource;
                graph.previousGraphTickIndex = previousGraph.tickIndex;
                graph.graphSource = this;
                foreach (Vertex v in graph.vertices.Values)
                {
                    if (v.weightHistory == null)
                        v.weightHistory = new uint[1];
                    else
                    {
                        uint[] weightHistory = v.weightHistory;
                        if (weightHistory.Length < historyDepth)
                            v.weightHistory = new uint[weightHistory.Length+1];
                        for (int i = v.weightHistory.Length-1; i > 0; i--)
                            v.weightHistory[i] = weightHistory[i-1];
                    }
                    v.weightHistory[0] = v.weight;
                    v.weight = v.incomingWeight = v.outgoingWeight = v.basicWeight = 0;
                    v.count = 0;
                    foreach (Edge e in v.outgoingEdges.Values)
                        e.weight = 0;
                }
            }
            if (graph.previousGraphTickIndex < graph.allocatedAfterTickIndex)
                graph.previousGraphTickIndex = graph.allocatedAfterTickIndex;
            graph.allocatedAfterTickIndex = allocatedAfterTickIndex;
            graph.allocatedBeforeTickIndex = allocatedBeforeTickIndex;

            GcType rootType = GetOrCreateGcType("<root>", 0);
            GcObject rootObject = GetOrCreateObject(0);
            rootObject.type = rootType;
            rootObject.references = roots;

            foreach (GcObject gcObject in idToObject.Values)
            {
                gcObject.level = int.MaxValue;
                gcObject.vertex = null;
            }

            AssignLevels(rootObject);

            AssignInterestLevels();

            int index = 0;
            foreach (GcType gcType in typeNameToGcType.Values)
            {
                gcType.index = index++;
            }
            GcType[] gcTypes = new GcType[index];
            typeHintTable = new int[index];

            foreach (GcType gcType in typeNameToGcType.Values)
            {
                gcTypes[gcType.index] = gcType;
            }

            Vertex[] pathFromRoot = new Vertex[32];
            foreach (GcObject gcObject in idToObject.Values)
            {
                if (   gcObject.level == int.MaxValue
                    || (gcObject.interestLevel & (InterestLevel.Interesting|InterestLevel.Display)) == InterestLevel.Ignore
                    || gcObject.allocTickIndex <= allocatedAfterTickIndex
                    || gcObject.allocTickIndex >= allocatedBeforeTickIndex)
                    continue;

                while (pathFromRoot.Length < gcObject.level+1)
                {
                    pathFromRoot = new Vertex[pathFromRoot.Length*2];
                }

                for (GcObject pathObject = gcObject; pathObject != null; pathObject = pathObject.parent)
                {
                    if ((pathObject.interestLevel & (InterestLevel.Interesting|InterestLevel.Display)) == InterestLevel.Ignore)
                        pathFromRoot[pathObject.level] = null;
                    else
                        pathFromRoot[pathObject.level] = FindVertex(pathObject, graph, options);
                }

                int levels = 0;
                for (int i = 0; i <= gcObject.level; i++)
                {
                    if (pathFromRoot[i] != null)
                        pathFromRoot[levels++] = pathFromRoot[i];
                }

                levels = Vertex.SqueezeOutRepetitions(pathFromRoot, levels);

                for (int i = 0; i < levels-1; i++)
                {
                    Vertex fromVertex = pathFromRoot[i];
                    Vertex toVertex = pathFromRoot[i+1];
                    Edge edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(gcObject.size);
                }

                Vertex thisVertex = pathFromRoot[levels-1];
                thisVertex.basicWeight += gcObject.size;
                thisVertex.count += 1;
            }

            foreach (Vertex v in graph.vertices.Values)
            {
                if (v.weight < v.outgoingWeight)
                    v.weight = v.outgoingWeight;
                if (v.weight < v.incomingWeight)
                    v.weight = v.incomingWeight;
                if (v.weightHistory == null)
                    v.weightHistory = new uint[1];
            }
            
            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }
    }
}
