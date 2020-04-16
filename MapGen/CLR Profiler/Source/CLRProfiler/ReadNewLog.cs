/* ==++==
 * 
 *   Copyright (c) Microsoft Corporation.  All rights reserved.
 * 
 * ==--==
 *
 * Class:  Histogram, ReadNewLog, SampleObjectTable, LiveObjectTable, etc.
 *
 * Description: Log file parser and various graph generators
 */

using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

namespace CLRProfiler
{
    internal class Histogram
    {
        internal int[] typeSizeStacktraceToCount;
        internal ReadNewLog readNewLog;

        internal Histogram(ReadNewLog readNewLog)
        {
            typeSizeStacktraceToCount = new int[10];
            this.readNewLog = readNewLog;
        }

        internal void AddObject(int typeSizeStacktraceIndex, int count)
        {
            while (typeSizeStacktraceIndex >= typeSizeStacktraceToCount.Length)
                typeSizeStacktraceToCount = ReadNewLog.GrowIntVector(typeSizeStacktraceToCount);
            typeSizeStacktraceToCount[typeSizeStacktraceIndex] += count;
        }

        internal bool Empty
        {
            get
            {
                foreach (int count in typeSizeStacktraceToCount)
                    if (count != 0)
                        return false;
                return true;
            }
        }

        internal int BuildVertexStack(int stackTraceIndex, Vertex[] funcVertex, ref Vertex[] vertexStack, int skipCount)
        {
            int[] stackTrace = readNewLog.stacktraceTable.IndexToStacktrace(stackTraceIndex);
                
            while (vertexStack.Length < stackTrace.Length + 3)
            {
                vertexStack = new Vertex[vertexStack.Length*2];
            }

            for (int i = skipCount; i < stackTrace.Length; i++)
            {
                vertexStack[i-skipCount] = funcVertex[stackTrace[i]];
            }

            return stackTrace.Length - skipCount;
        }

        internal void BuildAllocationTrace(Graph graph, int stackTraceIndex, int typeIndex, uint size, Vertex[] typeVertex, Vertex[] funcVertex, ref Vertex[] vertexStack)
        {
            int stackPtr = BuildVertexStack(stackTraceIndex, funcVertex, ref vertexStack, 2);

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;
            if ((typeVertex[typeIndex].interestLevel & InterestLevel.Interesting) == InterestLevel.Interesting
                && ReadNewLog.InterestingCallStack(vertexStack, stackPtr))
            {
                vertexStack[stackPtr] = typeVertex[typeIndex];
                stackPtr++;
                stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
                stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
                for (int i = 0; i < stackPtr; i++)
                {
                    fromVertex = toVertex;
                    toVertex = vertexStack[i];
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(size);
                }
                fromVertex = toVertex;
                toVertex = graph.BottomVertex;
                edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                edge.AddWeight(size);
            }
        }

        internal void BuildAssemblyTrace(Graph graph, int stackTraceIndex, Vertex assembly, Vertex typeVertex, Vertex[] funcVertex, ref Vertex[] vertexStack)
        {
            int stackPtr = BuildVertexStack(Math.Abs(stackTraceIndex), funcVertex, ref vertexStack, stackTraceIndex < 0 ? 2 : 0);

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;

            if(typeVertex != null)
            {
                vertexStack[stackPtr++] = typeVertex;
            }
            vertexStack[stackPtr++] = assembly;

            stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
            stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
            for (int i = 0; i < stackPtr; i++)
            {
                fromVertex = toVertex;
                toVertex = vertexStack[i];
                edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                edge.AddWeight(1);
            }
            fromVertex = toVertex;
            toVertex = graph.BottomVertex;
            edge = graph.FindOrCreateEdge(fromVertex, toVertex);
            edge.AddWeight(1);
        }

        internal void BuildCallTrace(Graph graph, int stackTraceIndex, Vertex[] funcVertex, ref Vertex[] vertexStack, int count)
        {
            int stackPtr = BuildVertexStack(stackTraceIndex, funcVertex, ref vertexStack, 0);

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;
            if (ReadNewLog.InterestingCallStack(vertexStack, stackPtr))
            {
                stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
                stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
                for (int i = 0; i < stackPtr; i++)
                {
                    fromVertex = toVertex;
                    toVertex = vertexStack[i];
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight((uint)count);
                }
            }
        }

        internal void BuildTypeVertices(Graph graph, ref Vertex[] typeVertex)
        {
            for (int i = 0; i < readNewLog.typeName.Length; i++)
            {
                string typeName = readNewLog.typeName[i];
                if (typeName != null)
                    readNewLog.AddTypeVertex(i, typeName, graph, ref typeVertex);
            }
        }

        internal int BuildAssemblyVertices(Graph graph, ref Vertex[] typeVertex)
        {
            int count = 0;
            foreach(string c in readNewLog.assemblies.Keys)
            {
                readNewLog.AddTypeVertex(count++, c, graph, ref typeVertex);
            }
            return count;
        }

        internal void BuildFuncVertices(Graph graph, ref Vertex[] funcVertex)
        {
            for (int i = 0; i < readNewLog.funcName.Length; i++)
            {
                string name = readNewLog.funcName[i];
                string signature = readNewLog.funcSignature[i];
                if (name != null && signature != null)
                    readNewLog.AddFunctionVertex(i, name, signature, graph, ref funcVertex);
            }
        }

        internal Graph BuildAllocationGraph()
        {
            Vertex[] typeVertex = new Vertex[1];
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.AllocationGraph;

            BuildTypeVertices(graph, ref typeVertex);
            BuildFuncVertices(graph, ref funcVertex);

            for (int i = 0; i < typeSizeStacktraceToCount.Length; i++)
            {
                if (typeSizeStacktraceToCount[i] > 0)
                {
                    int[] stacktrace = readNewLog.stacktraceTable.IndexToStacktrace(i);

                    int typeIndex = stacktrace[0];
                    uint size = (uint)(stacktrace[1]*typeSizeStacktraceToCount[i]);

                    BuildAllocationTrace(graph, i, typeIndex, size, typeVertex, funcVertex, ref vertexStack);
                }
            }

            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }

        internal Graph BuildAssemblyGraph()
        {
            Vertex[] assemblyVertex = new Vertex[1];
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] typeVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.AssemblyGraph;

            int count = BuildAssemblyVertices(graph, ref assemblyVertex);
            BuildTypeVertices(graph, ref typeVertex);
            BuildFuncVertices(graph, ref funcVertex);

            for(int i = 0; i < count; i++)
            {
                Vertex v = (Vertex)assemblyVertex[i], tv = null;

                string c = v.name;
                int stackid = (int)readNewLog.assemblies[c];
                if(stackid < 0)
                {
                    int[] stacktrace = readNewLog.stacktraceTable.IndexToStacktrace(-stackid);
                    tv = typeVertex[stacktrace[0]];
                }
                BuildAssemblyTrace(graph, stackid, v, tv, funcVertex, ref vertexStack);
            }

            foreach (Vertex v in graph.vertices.Values)
            {
                v.active = true;
            }
            graph.BottomVertex.active = false;
            return graph;
        }

        internal Graph BuildCallGraph()
        {
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.CallGraph;

            BuildFuncVertices(graph, ref funcVertex);

            for (int i = 0; i < typeSizeStacktraceToCount.Length; i++)
            {
                if (typeSizeStacktraceToCount[i] > 0)
                {
                    int[] stacktrace = readNewLog.stacktraceTable.IndexToStacktrace(i);

                    BuildCallTrace(graph, i, funcVertex, ref vertexStack, typeSizeStacktraceToCount[i]);
                }
            }

            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }
    }

    internal class SampleObjectTable
    {
        internal class SampleObject
        {
            internal int typeIndex;
            internal int allocTickIndex;
            internal SampleObject prev;

            internal SampleObject(int typeIndex, int allocTickIndex, SampleObject prev)
            {
                this.typeIndex = typeIndex;
                this.allocTickIndex = allocTickIndex;
                this.prev = prev;
            }
        }

        internal SampleObject[][] masterTable;
        internal ReadNewLog readNewLog;

        internal const int firstLevelShift = 20;
        internal const int firstLevelLength = 1<<(32-firstLevelShift);
        internal const int secondLevelShift = 10;
        internal const int secondLevelLength = 1<<(firstLevelShift-secondLevelShift);
        internal const int sampleGrain = 1<<secondLevelShift;
        internal int lastTickIndex;
        internal SampleObject gcTickList;
        internal SampleObject commentList;

        internal SampleObjectTable(ReadNewLog readNewLog)
        {
            masterTable = new SampleObject[firstLevelLength][];
            this.readNewLog = readNewLog;
            lastTickIndex = 0;
            gcTickList = null;
        }
            
        bool IsGoodSample(uint start, uint end)
        {
            // We want it as a sample if and only if it crosses a boundary
            return (start >> secondLevelShift) != (end >> secondLevelShift);
        }

        internal void RecordChange(uint start, uint end, int tickIndex, int typeIndex)
        {
            lastTickIndex = tickIndex;
            for (uint id = start; id < end; id += sampleGrain)
            {
                uint index = id >> firstLevelShift;
                SampleObject[] so = masterTable[index];
                if (so == null)
                {
                    so = new SampleObject[secondLevelLength];
                    masterTable[index] = so;
                }
                index = (id >> secondLevelShift) & (secondLevelLength-1);
                Debug.Assert(so[index] == null || so[index].allocTickIndex <= tickIndex);
                so[index] = new SampleObject(typeIndex, tickIndex, so[index]);
            }
        }

        internal void Insert(uint start, uint end, int tick, int typeIndex)
        {
            if (IsGoodSample(start, end))
                RecordChange(start, end, tick, typeIndex);
        }

        internal void Delete(uint start, uint end, int tick)
        {
            if (IsGoodSample(start, end))
                RecordChange(start, end, tick, 0);
        }

        internal void AddGcTick(int tickIndex, int gen)
        {
            lastTickIndex = tickIndex;

            gcTickList = new SampleObject(gen, tickIndex, gcTickList);
        }

        internal void RecordComment(int tickIndex, int commentIndex)
        {
            lastTickIndex = tickIndex;

            commentList = new SampleObject(commentIndex, tickIndex, commentList);
        }
    }

    internal class LiveObjectTable
    {
        internal struct LiveObject
        {
            internal uint id;
            internal uint size;
            internal int typeIndex;
            internal int typeSizeStacktraceIndex;
            internal int allocTickIndex;
        }

        class IntervalTable
        {
            class Interval
            {
                internal uint loAddr;
                internal uint hiAddr;
                internal Interval next;
                internal bool hadRelocations;
                internal bool justHadGc;

                internal Interval(uint loAddr, uint hiAddr, Interval next)
                {
                    this.loAddr = loAddr;
                    this.hiAddr = hiAddr;
                    this.next = next;
                }
            }

            const int allowableGap = 1024*1024;

            Interval liveRoot;
            Interval updateRoot;
            bool nullRelocationsSeen;

            LiveObjectTable liveObjectTable;

            internal IntervalTable(LiveObjectTable liveObjectTable)
            {
                liveRoot = null;
                this.liveObjectTable = liveObjectTable;
            }

            private Interval OverlappingInterval(Interval i)
            {
                for (Interval ii = liveRoot; ii != null; ii = ii.next)
                {
                    if (ii != i)
                    {
                        if (ii.hiAddr >= i.loAddr && ii.loAddr <= i.hiAddr)
                            return ii;
                    }
                }
                return null;
            }

            private void DeleteInterval(Interval i)
            {   
                Interval prevInterval = null;
                for (Interval ii = liveRoot; ii != null; ii = ii.next)
                {
                    prevInterval = ii;
                    if (ii == i)
                    {
                        if (prevInterval != null)
                            prevInterval.next = ii.next;
                        else
                            liveRoot = ii.next;
                        break;
                    }
                }
            }

            private void MergeInterval(Interval i)
            {
                Interval overlappingInterval = OverlappingInterval(i);
                i.loAddr = Math.Min(i.loAddr, overlappingInterval.loAddr);
                i.hiAddr = Math.Max(i.hiAddr, overlappingInterval.hiAddr);
                DeleteInterval(overlappingInterval);
            }

            internal bool AddObject(uint id, uint size, int allocTickIndex, SampleObjectTable sampleObjectTable)
            {
                size = (size + 3) & (uint.MaxValue - 3);
                Interval prevInterval = null;
                Interval bestInterval = null;
                Interval prevI = null;
                bool emptySpace = false;
                // look for the best interval to put this object in.
                for (Interval i = liveRoot; i != null; i = i.next)
                {
                    if (i.loAddr < id + size && id <= i.hiAddr + allowableGap)
                    {
                        if (bestInterval == null || bestInterval.loAddr < i.loAddr)
                        {
                            bestInterval = i;
                            prevInterval = prevI;
                        }
                    }
                    prevI = i;
                }
                if (bestInterval != null)
                {
                    if (bestInterval.loAddr > id)
                        bestInterval.loAddr = id;
                    if (id < bestInterval.hiAddr)
                    {
                        if (bestInterval.hadRelocations && bestInterval.justHadGc)
                        {
                            // Interval gets shortened
                            liveObjectTable.RemoveObjectRange(id, bestInterval.hiAddr - id, allocTickIndex, sampleObjectTable);
                            bestInterval.hiAddr = id + size;
                            bestInterval.justHadGc = false;
                        }
                    }
                    else
                    {
                        bestInterval.hiAddr = id + size;
                        emptySpace = true;
                    }                   
                    if (prevInterval != null)
                    {
                        // Move to front to speed up future searches.
                        prevInterval.next = bestInterval.next;
                        bestInterval.next = liveRoot;
                        liveRoot = bestInterval;
                    }
                    if (OverlappingInterval(bestInterval) != null)
                        MergeInterval(bestInterval);
                    return emptySpace;
                }
                liveRoot = new Interval(id, id + size, liveRoot);
                Debug.Assert(OverlappingInterval(liveRoot) == null);
                return emptySpace;
            }

            internal void Relocate(uint oldId, uint newId, uint length)
            {
                if (oldId == newId)
                    nullRelocationsSeen = true;

                if (updateRoot != null && updateRoot.hiAddr == newId)
                    updateRoot.hiAddr = newId + length;
                else
                    updateRoot = new Interval(newId, newId + length, updateRoot);

                for (Interval i = liveRoot; i != null; i = i.next)
                {
                    if (i.loAddr <= oldId && oldId < i.hiAddr)
                        i.hadRelocations = true;
                }
                Interval bestInterval = null;
                for (Interval i = liveRoot; i != null; i = i.next)
                {
                    if (i.loAddr <= newId + length && newId <= i.hiAddr + allowableGap)
                    {
                        if (bestInterval == null || bestInterval.loAddr < i.loAddr)
                            bestInterval = i;
                    }
                }
                if (bestInterval != null)
                {
                    if (bestInterval.hiAddr < newId + length)
                        bestInterval.hiAddr = newId + length;
                    if (bestInterval.loAddr > newId)
                        bestInterval.loAddr = newId;
                    if (OverlappingInterval(bestInterval) != null)
                        MergeInterval(bestInterval);
                }
                else
                {
                    liveRoot = new Interval(newId, newId + length, liveRoot);
                    Debug.Assert(OverlappingInterval(liveRoot) == null);
                }
            }

            private Interval SortIntervals(Interval root)
            {
                // using insertion sort for now...
                Interval next;
                Interval newRoot = null;
                for (Interval i = root; i != null; i = next)
                {
                    next = i.next;
                    Interval prev = null;
                    Interval ii;
                    for (ii = newRoot; ii != null; ii = ii.next)
                    {
                        if (i.loAddr < ii.loAddr)
                            break;
                        prev = ii;
                    }
                    if (prev == null)
                    {
                        i.next = newRoot;
                        newRoot = i;
                    }
                    else
                    {
                        i.next = ii;
                        prev.next = i;
                    }
                }
                return newRoot;
            }

            private void RemoveRange(uint loAddr, uint hiAddr, int tickIndex, SampleObjectTable sampleObjectTable)
            {
                Interval next;
                for (Interval i = liveRoot; i != null; i = next)
                {
                    next = i.next;
                    uint lo = Math.Max(loAddr, i.loAddr);
                    uint hi = Math.Min(hiAddr, i.hiAddr);
                    if (lo >= hi)
                        continue;
                    liveObjectTable.RemoveObjectRange(lo, hi - lo, tickIndex, sampleObjectTable);
                    if (i.hiAddr == hi)
                    {
                        if (i.loAddr == lo)
                            DeleteInterval(i);
                        else
                            i.hiAddr = lo;
                    }
                }
            }

            internal void RecordGc(int tickIndex, SampleObjectTable sampleObjectTable, bool simpleForm)
            {
                if (simpleForm && nullRelocationsSeen)
                {
                    // in this case assume anything not reported is dead
                    updateRoot = SortIntervals(updateRoot);
                    uint prevHiAddr = 0;
                    for (Interval i = updateRoot; i != null; i = i.next)
                    {
                        if (prevHiAddr < i.loAddr)
                        {
                            RemoveRange(prevHiAddr, i.loAddr, tickIndex, sampleObjectTable);
                        }
                        if (prevHiAddr < i.hiAddr)
                            prevHiAddr = i.hiAddr;
                    }
                    RemoveRange(prevHiAddr, uint.MaxValue, tickIndex, sampleObjectTable);
                    updateRoot = null;
                }
                else
                {
                    for (Interval i = liveRoot; i != null; i = i.next)
                        i.justHadGc = true;
                }
                nullRelocationsSeen = false;
            }
        }

        IntervalTable intervalTable;
        internal ReadNewLog readNewLog;
        internal int lastTickIndex;
        private long lastPos;

        const int alignShift = 2;
        const int firstLevelShift = 15;
        const int firstLevelLength = 1<<(32-alignShift-firstLevelShift);
        const int secondLevelLength = 1<<firstLevelShift;
        const int secondLevelMask = secondLevelLength-1;

        ushort[][] firstLevelTable;

        internal LiveObjectTable(ReadNewLog readNewLog)
        {
            firstLevelTable = new ushort[firstLevelLength][];
            intervalTable = new IntervalTable(this);
            this.readNewLog = readNewLog;
            lastGcGen0Count = 0;
            lastGcGen1Count = 0;
            lastGcGen2Count = 0;
            lastTickIndex = 0;
            lastPos = 0;
        }

        internal uint FindObjectBackward(uint id)
        {
            id >>= alignShift;
            uint i = id >> firstLevelShift;
            uint j = id & secondLevelMask;
            while (i != uint.MaxValue)
            {
                ushort[] secondLevelTable = firstLevelTable[i];
                if (secondLevelTable != null)
                {
                    while (j != uint.MaxValue)
                    {
                        if ((secondLevelTable[j] & 0x8000) != 0)
                            break;
                        j--;
                    }
                    if (j != uint.MaxValue)
                        break;
                }
                j = secondLevelLength - 1;
                i--;
            }
            if (i == uint.MaxValue)
                return 0;
            else
                return ((i<<firstLevelShift) + j) << alignShift;
        }

        uint FindObjectForward(uint startId, uint endId)
        {
            startId >>= alignShift;
            endId >>= alignShift;
            uint i = startId >> firstLevelShift;
            uint iEnd = endId >> firstLevelShift;
            uint j = startId & secondLevelMask;
            uint jEnd = endId & secondLevelMask;
            while (i <= iEnd)
            {
                ushort[] secondLevelTable = firstLevelTable[i];
                if (secondLevelTable != null)
                {
                    while (j < secondLevelLength && (j <= jEnd || i <= iEnd))
                    {
                        if ((secondLevelTable[j] & 0x8000) != 0)
                            break;
                        j++;
                    }
                    if (j < secondLevelLength)
                        break;
                }
                j = 0;
                i++;
            }
            if (i > iEnd)
                return uint.MaxValue;
            else
                return ((i<<firstLevelShift) + j) << alignShift;
        }

        internal void GetNextObject(uint startId, uint endId, out LiveObject o)
        {
            uint id = FindObjectForward(startId, endId);
            o.id = id;
            id >>= alignShift;
            uint i = id >> firstLevelShift;
            uint j = id & secondLevelMask;
            ushort[] secondLevelTable = firstLevelTable[i];
            if (secondLevelTable != null)
            {
                ushort u1 = secondLevelTable[j];
                if ((u1 & 0x8000) != 0)
                {
                    j++;
                    if (j >= secondLevelLength)
                    {
                        j = 0;
                        i++;
                        secondLevelTable = firstLevelTable[i];
                    }
                    ushort u2 = secondLevelTable[j];
                    j++;
                    if (j >= secondLevelLength)
                    {
                        j = 0;
                        i++;
                        secondLevelTable = firstLevelTable[i];
                    }
                    ushort u3 = secondLevelTable[j];

                    o.allocTickIndex = (u2 >> 7) + (u3 << 8);

                    o.typeSizeStacktraceIndex = (u1 & 0x7fff) + ((u2 & 0x7f) << 15);

                    int[] stacktrace = readNewLog.stacktraceTable.IndexToStacktrace(o.typeSizeStacktraceIndex);

                    o.typeIndex = stacktrace[0];
                    o.size = (uint)stacktrace[1];

                    return;
                }
            }
            o.size = 0;
            o.allocTickIndex = o.typeIndex = o.typeSizeStacktraceIndex = 0;
        }

        void Write3WordsAt(uint id, ushort u1, ushort u2, ushort u3)
        {
            id >>= alignShift;
            uint i = id >> firstLevelShift;
            uint j = id & secondLevelMask;
            ushort[] secondLevelTable = firstLevelTable[i];
            if (secondLevelTable == null)
            {
                secondLevelTable = new ushort[secondLevelLength];
                firstLevelTable[i] = secondLevelTable;
            }
            secondLevelTable[j] = u1;
            j++;
            if (j >= secondLevelLength)
            {
                j = 0;
                i++;
                secondLevelTable = firstLevelTable[i];
                if (secondLevelTable == null)
                {
                    secondLevelTable = new ushort[secondLevelLength];
                    firstLevelTable[i] = secondLevelTable;
                }
            }
            secondLevelTable[j] = u2;
            j++;
            if (j >= secondLevelLength)
            {
                j = 0;
                i++;
                secondLevelTable = firstLevelTable[i];
                if (secondLevelTable == null)
                {
                    secondLevelTable = new ushort[secondLevelLength];
                    firstLevelTable[i] = secondLevelTable;
                }
            }
            secondLevelTable[j] = u3;
        }

        internal void Zero(uint id, uint size)
        {
            uint count = ((size + 3) & (uint.MaxValue - 3))/4;
            id >>= alignShift;
            uint i = id >> firstLevelShift;
            uint j = id & secondLevelMask;
            ushort[] secondLevelTable = firstLevelTable[i];
            if (secondLevelTable == null)
            {
                secondLevelTable = new ushort[secondLevelLength];
                firstLevelTable[i] = secondLevelTable;
            }
            while (count > 0)
            {
                if (j + count <= secondLevelLength)
                {
                    while (count > 0)
                    {
                        secondLevelTable[j] = 0;
                        count--;
                        j++;
                    }
                }
                else
                {
                    while (j < secondLevelLength)
                    {
                        secondLevelTable[j] = 0;
                        count--;
                        j++;
                    }
                    j = 0;
                    i++;
                    secondLevelTable = firstLevelTable[i];
                    if (secondLevelTable == null)
                    {
                        secondLevelTable = new ushort[secondLevelLength];
                        firstLevelTable[i] = secondLevelTable;
                    }
                }
            }
        }

        internal bool CanReadObjectBackCorrectly(uint id, uint size, int typeSizeStacktraceIndex, int allocTickIndex)
        {
            LiveObject o;
            GetNextObject(id, id + size, out o);
            return o.id == id && o.typeSizeStacktraceIndex == typeSizeStacktraceIndex && o.allocTickIndex == allocTickIndex;
        }

        internal void InsertObject(uint id, int typeSizeStacktraceIndex, int allocTickIndex, int nowTickIndex, bool newAlloc, SampleObjectTable sampleObjectTable)
        {
            if (lastPos >= readNewLog.pos && newAlloc)
                return;
            lastPos = readNewLog.pos;

            lastTickIndex = nowTickIndex;
            int[] stacktrace = readNewLog.stacktraceTable.IndexToStacktrace(typeSizeStacktraceIndex);
            int typeIndex = stacktrace[0];
            uint size = (uint)stacktrace[1];
            bool emptySpace = false;
            if (newAlloc)
                emptySpace = intervalTable.AddObject(id, (uint)size, allocTickIndex, sampleObjectTable);
            if (!emptySpace)
            {
                uint prevId = FindObjectBackward(id-4);
                LiveObject o;
                GetNextObject(prevId, id, out o);
                if (o.id < id && o.id + o.size > id)
                {
                    Zero(o.id, id - o.id);
                }
            }
            Debug.Assert(FindObjectBackward(id-4)+12 <= id);
            if (size >= 12)
            {
                ushort u1 = (ushort)(typeSizeStacktraceIndex | 0x8000);
                ushort u2 = (ushort)((typeSizeStacktraceIndex >> 15) | ((allocTickIndex & 0xff) << 7));
                ushort u3 = (ushort)(allocTickIndex >> 8);
                Write3WordsAt(id, u1, u2, u3);
                if (!emptySpace)
                    Zero(id + 12, size - 12);
                Debug.Assert(CanReadObjectBackCorrectly(id, size, typeSizeStacktraceIndex, allocTickIndex));
            }
            if (sampleObjectTable != null)
                sampleObjectTable.Insert(id, id + size, nowTickIndex, typeIndex);
        }

        void RemoveObjectRange(uint firstId, uint length, int tickIndex, SampleObjectTable sampleObjectTable)
        {
            uint lastId = firstId + length;

            if (sampleObjectTable != null)
                sampleObjectTable.Delete(firstId, lastId, tickIndex);

            Zero(firstId, length);
        }

        internal void UpdateObjects(Histogram relocatedHistogram, uint oldId, uint newId, uint length, int tickIndex, SampleObjectTable sampleObjectTable)
        {
            if (lastPos >= readNewLog.pos)
                return;
            lastPos = readNewLog.pos;

            lastTickIndex = tickIndex;
            intervalTable.Relocate(oldId, newId, length);

            if (oldId == newId)
                return;
            uint nextId;
            uint lastId = oldId + length;
            LiveObject o;
            for (GetNextObject(oldId, lastId, out o); o.id < lastId; GetNextObject(nextId, lastId, out o))
            {
                nextId = o.id + o.size;
                uint offset = o.id - oldId;
                if (sampleObjectTable != null)
                    sampleObjectTable.Insert(o.id, o.id + o.size, tickIndex, 0);
                Zero(o.id, o.size);
                InsertObject(newId + offset, o.typeSizeStacktraceIndex, o.allocTickIndex, tickIndex, false, sampleObjectTable);
                if (relocatedHistogram != null)
                    relocatedHistogram.AddObject(o.typeSizeStacktraceIndex, 1);
            }
        }

        private int lastGcGen0Count;
        private int lastGcGen1Count;
        private int lastGcGen2Count;

        internal int gen1LimitTickIndex;
        internal int gen2LimitTickIndex;

        internal void RecordGc(int tickIndex, int gen, SampleObjectTable sampleObjectTable, bool simpleForm)
        {
            lastTickIndex = tickIndex;

            if (sampleObjectTable != null)
                sampleObjectTable.AddGcTick(tickIndex, gen);
    
            intervalTable.RecordGc(tickIndex, sampleObjectTable, simpleForm);

            if (gen >= 1)
                gen2LimitTickIndex = gen1LimitTickIndex;
            gen1LimitTickIndex = tickIndex;
        }

        internal void RecordGc(int tickIndex, int gcGen0Count, int gcGen1Count, int gcGen2Count, SampleObjectTable sampleObjectTable)
        {
            int gen = 0;
            if (gcGen2Count != lastGcGen2Count)
                gen = 2;
            else if (gcGen1Count != lastGcGen1Count)
                gen = 1;

            RecordGc(tickIndex, gen, sampleObjectTable, false);

            lastGcGen0Count = gcGen0Count;
            lastGcGen1Count = gcGen1Count;
            lastGcGen2Count = gcGen2Count;
        }
    }

    internal class StacktraceTable
    {
        private int[][] stacktraceTable;
        private int maxID = -1;

        internal StacktraceTable()
        {
            stacktraceTable = new int[1000][];
            stacktraceTable[0] = new int[0];
        }

        internal void Add(int id, int[] stack, int length)
        {
            Add( id, stack, 0, length );
        }

        internal void Add(int id, int[] stack, int start, int length)
        {
            while (stacktraceTable.Length <= id)
            {
                int[][] newStacktraceTable = new int[stacktraceTable.Length*2][];
                for (int i = 0; i < stacktraceTable.Length; i++)
                    newStacktraceTable[i] = stacktraceTable[i];
                stacktraceTable = newStacktraceTable;
            }

            int[] stacktrace = new int[length];
            for (int i = 0; i < stacktrace.Length; i++)
                stacktrace[i] = stack[start++];

            stacktraceTable[id] = stacktrace;

            if (id > maxID)
            {
                maxID = id;
            }
        }

        internal int[] IndexToStacktrace(int index)
        {
            return stacktraceTable[index];
        }

        internal void FreeEntries( int firstIndex )
        {
            maxID = firstIndex;
        }

        internal int Length 
        {
            get 
            { 
                return maxID + 1;
            }
        }
    }

    internal struct TimePos
    {
        internal double time;
        internal long pos;

        internal TimePos(double time, long pos)
        {
            this.time = time;
            this.pos = pos;
        }
    }

    internal class FunctionList
    {
        internal class FunctionDescriptor
        {
            internal FunctionDescriptor(int functionId, int funcCallStack, uint funcSize, int funcModule)
            {
                this.functionId = functionId;
                this.funcCallStack = funcCallStack;
                this.funcSize = funcSize;
                this.funcModule = funcModule;
            }

            internal int functionId;
            internal int funcCallStack;
            internal uint funcSize;
            internal int funcModule;
        }
    
        ReadNewLog readNewLog;
        ArrayList functionList;

        internal FunctionList(ReadNewLog readNewLog)
        {
            this.readNewLog = readNewLog;
            this.functionList = new ArrayList();
        }

        internal void Add(int functionId, int funcCallStack, uint funcSize, int funcModule)
        {
            functionList.Add(new FunctionDescriptor(functionId, funcCallStack, funcSize, funcModule));
        }

        internal bool Empty
        {
            get
            {
                return functionList.Count == 0;
            }
        }

        void BuildFuncVertices(Graph graph, ref Vertex[] funcVertex)
        {
            for (int i = 0; i < readNewLog.funcName.Length; i++)
            {
                string name = readNewLog.funcName[i];
                string signature = readNewLog.funcSignature[i];
                if (name != null && signature != null)
                    readNewLog.AddFunctionVertex(i, name, signature, graph, ref funcVertex);
            }
        }

        int BuildVertexStack(int stackTraceIndex, Vertex[] funcVertex, ref Vertex[] vertexStack, int skipCount)
        {
            int[] stackTrace = readNewLog.stacktraceTable.IndexToStacktrace(stackTraceIndex);
                
            while (vertexStack.Length < stackTrace.Length+1)
                vertexStack = new Vertex[vertexStack.Length*2];

            for (int i = skipCount; i < stackTrace.Length; i++)
                vertexStack[i-skipCount] = funcVertex[stackTrace[i]];

            return stackTrace.Length - skipCount;
        }

        void BuildFunctionTrace(Graph graph, int stackTraceIndex, int funcIndex, uint size, Vertex[] funcVertex, ref Vertex[] vertexStack)
        {
            int stackPtr = BuildVertexStack(stackTraceIndex, funcVertex, ref vertexStack, 0);

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;
            if ((funcVertex[funcIndex].interestLevel & InterestLevel.Interesting) == InterestLevel.Interesting
                && ReadNewLog.InterestingCallStack(vertexStack, stackPtr))
            {
                vertexStack[stackPtr] = funcVertex[funcIndex];
                stackPtr++;
                stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
                stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
                for (int i = 0; i < stackPtr; i++)
                {
                    fromVertex = toVertex;
                    toVertex = vertexStack[i];
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(size);
                }
                fromVertex = toVertex;
                toVertex = graph.BottomVertex;
                edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                edge.AddWeight(size);
            }
        }

        internal Graph BuildFunctionGraph()
        {
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.FunctionGraph;

            BuildFuncVertices(graph, ref funcVertex);

            foreach (FunctionDescriptor fd in functionList)
            {
                BuildFunctionTrace(graph, fd.funcCallStack, fd.functionId, fd.funcSize, funcVertex, ref vertexStack);
            }

            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }

        void BuildModVertices(Graph graph, ref Vertex[] modVertex)
        {
            for (int i = 0; i < readNewLog.modBasicName.Length; i++)
            {
                string basicName = readNewLog.modBasicName[i];
                string fullName = readNewLog.modFullName[i];
                if (basicName != null && fullName != null)
                {
                    readNewLog.AddFunctionVertex(i, basicName, fullName, graph, ref modVertex);
                    modVertex[i].basicName = basicName;
                    modVertex[i].basicSignature = fullName;
                }
            }
        }

        int FunctionsInSameModule(int modIndex, int stackTraceIndex)
        {
            int[] stackTrace = readNewLog.stacktraceTable.IndexToStacktrace(stackTraceIndex);
            int result = 0;
            for (int i = stackTrace.Length - 1; i >= 0; i--)
            {
                int funcIndex = stackTrace[i];
                if (readNewLog.funcModule[funcIndex] == modIndex)
                    result++;
                else
                    break;
            }
            return result;
        }

        void BuildModuleTrace(Graph graph, int stackTraceIndex, int modIndex, uint size, Vertex[] funcVertex, Vertex[] modVertex, ref Vertex[] vertexStack)
        {
            int functionsToSkip = FunctionsInSameModule(modIndex, stackTraceIndex);
            int stackPtr = BuildVertexStack(stackTraceIndex, funcVertex, ref vertexStack, 0) - functionsToSkip;

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;
            if (ReadNewLog.InterestingCallStack(vertexStack, stackPtr))
            {
                vertexStack[stackPtr] = modVertex[modIndex];
                stackPtr++;
                stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
                stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
                for (int i = 0; i < stackPtr; i++)
                {
                    fromVertex = toVertex;
                    toVertex = vertexStack[i];
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(size);
                }
                fromVertex = toVertex;
                toVertex = graph.BottomVertex;
                edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                edge.AddWeight(size);
            }
        }

        internal Graph BuildModuleGraph()
        {
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];
            Vertex[] modVertex = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.ModuleGraph;

            BuildFuncVertices(graph, ref funcVertex);
            BuildModVertices(graph, ref modVertex);

            foreach (FunctionDescriptor fd in functionList)
            {
                BuildModuleTrace(graph, fd.funcCallStack, fd.funcModule, fd.funcSize, funcVertex, modVertex, ref vertexStack);
            }

            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }

        string ClassNameOfFunc(int funcIndex)
        {
            string funcName = readNewLog.funcName[funcIndex];
            int colonColonIndex = funcName.IndexOf("::");
            if (colonColonIndex > 0)
                return funcName.Substring(0, colonColonIndex);
            else
                return funcName;
        }

        int FunctionsInSameClass(string className, int stackTraceIndex)
        {
            int[] stackTrace = readNewLog.stacktraceTable.IndexToStacktrace(stackTraceIndex);
            int result = 0;
            for (int i = stackTrace.Length - 1; i >= 0; i--)
            {
                int funcIndex = stackTrace[i];
                if (ClassNameOfFunc(funcIndex) == className)
                    result++;
                else
                    break;
            }
            return result;
        }

        void BuildClassTrace(Graph graph, int stackTraceIndex, int funcIndex, uint size, Vertex[] funcVertex, ref Vertex[] vertexStack)
        {
            string className = ClassNameOfFunc(funcIndex);
            int functionsToSkip = FunctionsInSameClass(className, stackTraceIndex);
            int stackPtr = BuildVertexStack(stackTraceIndex, funcVertex, ref vertexStack, 0) - functionsToSkip;

            Vertex toVertex = graph.TopVertex;
            Vertex fromVertex;
            Edge edge;
            if (ReadNewLog.InterestingCallStack(vertexStack, stackPtr))
            {
                vertexStack[stackPtr] = graph.FindOrCreateVertex(className, null, null);
                vertexStack[stackPtr].interestLevel = FilterForm.InterestLevelOfMethodName(className);
                stackPtr++;
                stackPtr = ReadNewLog.FilterVertices(vertexStack, stackPtr);
                stackPtr = Vertex.SqueezeOutRepetitions(vertexStack, stackPtr);
                for (int i = 0; i < stackPtr; i++)
                {
                    fromVertex = toVertex;
                    toVertex = vertexStack[i];
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(size);
                }
                if (toVertex != graph.TopVertex)
                {
                    fromVertex = toVertex;
                    toVertex = graph.BottomVertex;
                    edge = graph.FindOrCreateEdge(fromVertex, toVertex);
                    edge.AddWeight(size);
                }
            }
        }

        internal Graph BuildClassGraph()
        {
            Vertex[] funcVertex = new Vertex[1];
            Vertex[] vertexStack = new Vertex[1];

            Graph graph = new Graph(this);
            graph.graphType = Graph.GraphType.ClassGraph;

            BuildFuncVertices(graph, ref funcVertex);

            foreach (FunctionDescriptor fd in functionList)
            {
                BuildClassTrace(graph, fd.funcCallStack, fd.functionId, fd.funcSize, funcVertex, ref vertexStack);
            }

            foreach (Vertex v in graph.vertices.Values)
                v.active = true;
            graph.BottomVertex.active = false;

            return graph;
        }
    }

    internal class ReadLogResult
    {
        internal Histogram allocatedHistogram;
        internal Histogram relocatedHistogram;
        internal Histogram callstackHistogram;
        internal LiveObjectTable liveObjectTable;
        internal SampleObjectTable sampleObjectTable;
        internal ObjectGraph objectGraph;
        internal FunctionList functionList;
        internal bool hadAllocInfo, hadCallInfo;
    }

    internal class ReadNewLog
    {
        internal ReadNewLog(string fileName)
        {
            //
            // TODO: Add constructor logic here
            //
            assemblies = new Hashtable();
            assembliesJustLoaded = new Hashtable();
            typeName = new string[1000];
            funcName = new string[1000];
            funcSignature = new string[1000];
            funcModule = new int[1000];
            modBasicName = new string[10];
            modFullName = new string[10];
            finalizableTypes = new Hashtable();

            this.fileName = fileName;
        }

        internal StacktraceTable stacktraceTable;
        internal string fileName;

        StreamReader r;
        byte[] buffer;
        int bufPos;
        int bufLevel;
        int c;
        int line;
        internal long pos;
        long lastLineStartPos;
        internal Hashtable /* <threadId, ArrayList<string> > */ assembliesJustLoaded;
        internal Hashtable /* <string [assembly name], int [stack id]> */ assemblies;
        internal string[] typeName;
        internal string[] funcName;
        internal string[] funcSignature;
        internal int[] funcModule;
        internal string[] modBasicName;
        internal string[] modFullName;
        internal string[] commentTable;
        internal Hashtable finalizableTypes;

        void EnsureVertexCapacity(int id, ref Vertex[] vertexArray)
        {
            Debug.Assert(id >= 0);
            if (id < vertexArray.Length)
                return;
            int newLength = vertexArray.Length*2;
            if (newLength <= id)
                newLength = id + 1;
            Vertex[] newVertexArray = new Vertex[newLength];
            Array.Copy(vertexArray, 0, newVertexArray, 0, vertexArray.Length);
            vertexArray = newVertexArray;
        }

        void EnsureStringCapacity(int id, ref string[] stringArray)
        {
            Debug.Assert(id >= 0);
            if (id < stringArray.Length)
                return;
            int newLength = stringArray.Length*2;
            if (newLength <= id)
                newLength = id + 1;
            string[] newStringArray = new string[newLength];
            Array.Copy(stringArray, 0, newStringArray, 0, stringArray.Length);
            stringArray = newStringArray;
        }

        void EnsureIntCapacity(int id, ref int[] intArray)
        {
            Debug.Assert(id >= 0);
            if (id < intArray.Length)
                return;
            int newLength = intArray.Length*2;
            if (newLength <= id)
                newLength = id + 1;
            int[] newIntArray = new int[newLength];
            Array.Copy(intArray, 0, newIntArray, 0, intArray.Length);
            intArray = newIntArray;
        }

        internal void AddTypeVertex(int typeId, string typeName, Graph graph, ref Vertex[] typeVertex)
        {
            EnsureVertexCapacity(typeId, ref typeVertex);
            typeVertex[typeId] = graph.FindOrCreateVertex(typeName, null, null);
            typeVertex[typeId].interestLevel = FilterForm.InterestLevelOfTypeName(typeName, finalizableTypes[typeId] != null);
        }

        internal void AddFunctionVertex(int funcId, string functionName, string signature, Graph graph, ref Vertex[] funcVertex)
        {
            EnsureVertexCapacity(funcId, ref funcVertex);
            int moduleId = funcModule[funcId];
            string moduleName = null;
            if (moduleId >= 0)
                moduleName = modBasicName[moduleId];
            funcVertex[funcId] = graph.FindOrCreateVertex(functionName, signature, moduleName);
            funcVertex[funcId].interestLevel = FilterForm.InterestLevelOfMethodName(functionName);
        }

        void AddTypeName(int typeId, string typeName)
        {
            EnsureStringCapacity(typeId, ref this.typeName);
            this.typeName[typeId] = typeName;
        }

        int FillBuffer()
        {
            bufPos = 0;
            bufLevel = r.BaseStream.Read(buffer, 0, buffer.Length);
            if (bufPos < bufLevel)
                return buffer[bufPos++];
            else
                return -1;
        }

        internal int ReadChar()
        {
            pos++;
            if (bufPos < bufLevel)
                return buffer[bufPos++];
            else
                return FillBuffer();
        }

        int ReadHex()
        {
            int value = 0;
            while (true)
            {
                c = ReadChar();
                int digit = c;
                if (digit >= '0' && digit <= '9')
                    digit -= '0';
                else if (digit >= 'a' && digit <= 'f')
                    digit -= 'a' - 10;
                else if (digit >= 'A' && digit <= 'F')
                    digit -= 'A' - 10;
                else
                    return value;
                value = value*16 + digit;
            }
        }

        int ReadInt()
        {
            while (c == ' ' || c == '\t')
                c = ReadChar();
            bool negative = false;
            if (c == '-')
            {
                negative = true;
                c = ReadChar();
            }
            if (c >= '0' && c <= '9')
            {
                int value = 0;
                if (c == '0')
                {
                    c = ReadChar();
                    if (c == 'x' || c == 'X')
                        value = ReadHex();
                }
                while (c >= '0' && c <= '9')
                {
                    value = value*10 + c - '0';
                    c = ReadChar();
                }

                if (negative)
                    value = -value;
                return value;
            }
            else
            {
                return -1;
            }
        }

        uint ReadUInt()
        {
            return (uint)ReadInt();
        }

        int ForcePosInt()
        {
            int value = ReadInt();
            if (value >= 0)
                return value;
            else
                throw new Exception(string.Format("Bad format in log file {0} line {1}", fileName, line));
        }

        internal static int[] GrowIntVector(int[] vector)
        {
            int[] newVector = new int[vector.Length*2];
            for (int i = 0; i < vector.Length; i++)
                newVector[i] = vector[i];
            return newVector;
        }

        internal static uint[] GrowUIntVector(uint[] vector)
        {
            uint[] newVector = new uint[vector.Length*2];
            for (int i = 0; i < vector.Length; i++)
                newVector[i] = vector[i];
            return newVector;
        }

        internal static bool InterestingCallStack(Vertex[] vertexStack, int stackPtr)
        {
            if (stackPtr == 0)
                return FilterForm.methodFilter == "";
            if ((vertexStack[stackPtr-1].interestLevel & InterestLevel.Interesting) == InterestLevel.Interesting)
                return true;
            for (int i = stackPtr-2; i >= 0; i--)
            {
                switch (vertexStack[i].interestLevel & InterestLevel.InterestingChildren)
                {
                    case    InterestLevel.Ignore:
                        break;

                    case    InterestLevel.InterestingChildren:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

        internal static int FilterVertices(Vertex[] vertexStack, int stackPtr)
        {
            bool display = false;
            for (int i = 0; i < stackPtr; i++)
            {
                Vertex vertex = vertexStack[i];
                switch (vertex.interestLevel & InterestLevel.InterestingChildren)
                {
                    case    InterestLevel.Ignore:
                        if (display)
                            vertex.interestLevel |= InterestLevel.Display;
                        break;

                    case    InterestLevel.InterestingChildren:
                        display = true;
                        break;

                    default:
                        display = false;
                        break;
                }
            }
            display = false;
            for (int i = stackPtr-1; i >= 0; i--)
            {
                Vertex vertex = vertexStack[i];
                switch (vertex.interestLevel & InterestLevel.InterestingParents)
                {
                    case    InterestLevel.Ignore:
                        if (display)
                            vertex.interestLevel |= InterestLevel.Display;
                        break;

                    case    InterestLevel.InterestingParents:
                        display = true;
                        break;

                    default:
                        display = false;
                        break;
                }
            }
            int newStackPtr = 0;
            for (int i = 0; i < stackPtr; i++)
            {
                Vertex vertex = vertexStack[i];
                if ((vertex.interestLevel & (InterestLevel.Display|InterestLevel.Interesting)) != InterestLevel.Ignore)
                {
                    vertexStack[newStackPtr++] = vertex;
                    vertex.interestLevel &= ~InterestLevel.Display;
                }
            }
            return newStackPtr;
        }

        TimePos[] timePos;
        int timePosCount, timePosIndex;
        const int maxTimePosCount = (1<<23)-1; // ~8,000,000 entries

        void GrowTimePos()
        {
            TimePos[] newTimePos = new TimePos[2*timePos.Length];
            for (int i = 0; i < timePos.Length; i++)
                newTimePos[i] = timePos[i];
            timePos = newTimePos;
        }

        int AddTimePos(int tick, long pos)
        {
            double time = tick*0.001;
            
            // The time stamps can not always be taken at face value.
            // The two problems we try to fix here are:
            // - the time may wrap around (after about 50 days).
            // - on some MP machines, different cpus could drift apart
            // We solve the first problem by adding 2**32*0.001 if the
            // time appears to jump backwards by more than 2**31*0.001.
            // We "solve" the second problem by ignoring time stamps
            // that still jump backward in time.
            double lastTime = 0.0;
            if (timePosIndex > 0)
                lastTime = timePos[timePosIndex-1].time;
            // correct possible wraparound
            while (time + (1L<<31)*0.001 < lastTime)
                time += (1L<<32)*0.001;

            // ignore times that jump backwards
            if (time < lastTime)
                return timePosIndex;

            while (timePosCount >= timePos.Length)
                GrowTimePos();

            // we have only 23 bits to encode allocation time.
            // to avoid running out for long running measurements, we decrease time resolution
            // as we chew up slots. below algorithm uses 1 millisecond resolution for the first
            // million slots, 2 milliseconds for the second million etc. this gives about
            // 2 million seconds time range or 23 days. This is if we really have a time stamp
            // every millisecond - if not, the range is much larger...
            double minimumTimeInc = 0.000999*(1<<timePosIndex/(maxTimePosCount/8));
            if (timePosCount < maxTimePosCount && (time - lastTime >= minimumTimeInc))
            {
                if (timePosIndex < timePosCount)
                {
                    // This is the case where we read the file again for whatever reason
                    Debug.Assert(timePos[timePosIndex].time == time && timePos[timePosIndex].pos == pos);
                    return timePosIndex++;
                }
                else
                {
                    timePos[timePosCount] = new TimePos(time, pos);
                    timePosIndex++;
                    return timePosCount++;
                }
            }
            else
                return timePosIndex;
        }

        internal double TickIndexToTime(int tickIndex)
        {
            return timePos[tickIndex].time;
        }

        internal long TickIndexToPos(int tickIndex)
        {
            return timePos[tickIndex].pos;
        }

        internal int TimeToTickIndex(double time)
        {
            int l = 0;
            int r = timePosCount-1;
            if (time < timePos[l].time)
                return l;
            if (timePos[r].time <= time)
                return r;

            // binary search - loop invariant is timePos[l].time <= time && time < timePos[r].time
            // loop terminates because loop condition implies l < m < r and so the interval
            // shrinks on each iteration
            while (l + 1 < r)
            {
                int m = (l + r) / 2;
                if (time < timePos[m].time)
                {
                    r = m;
                }
                else
                {
                    l = m;
                }
            }

            // we still have the loop invariant timePos[l].time <= time && time < timePos[r].time
            // now we just return the index that gives the closer match.
            if (time - timePos[l].time < timePos[r].time - time)
                return l;
            else
                return r;
        }

        internal void ReadFile(long startFileOffset, long endFileOffset, ReadLogResult readLogResult)
        {
            Form2 progressForm = new Form2();
            progressForm.Text = string.Format("Progress loading {0}", fileName);
            progressForm.Visible = true;
            progressForm.setProgress(0);
            if (stacktraceTable == null)
                stacktraceTable = new StacktraceTable();
            if (timePos == null)
                timePos = new TimePos[1000];
            AddTypeName(0, "Free Space");
            try
            {
                Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                r = new StreamReader(s);
                for (timePosIndex = timePosCount; timePosIndex > 0; timePosIndex--)
                    if (timePos[timePosIndex-1].pos <= startFileOffset)
                        break;
                if (timePosIndex <= 1)
                {
                    pos = 0;
                    timePosIndex = 1;
                }
                else
                {
                    timePosIndex--;
                    pos = timePos[timePosIndex].pos;
                }
                if (timePosCount == 0)
                {
                    timePos[0] = new TimePos(0.0, 0);
                    timePosCount = timePosIndex = 1;
                }
                s.Position = pos;
                buffer = new byte[4096];
                bufPos = 0;
                bufLevel = 0;
                int maxProgress = (int)(r.BaseStream.Length/1024);
                progressForm.setMaximum(maxProgress);
                this.fileName = fileName;
                line = 1;
                StringBuilder sb = new StringBuilder();
                uint[] uintStack = new uint[1000];
                int[] intStack = new int[1000];
                int stackPtr = 0;
                c = ReadChar();
                bool thisIsR = false, previousWasR;
                int lastTickIndex = 0;
                int commentIndex = 0;
                while (c != -1)
                {
                    if (pos > endFileOffset)
                        break;
                    if ((line % 1024) == 0)
                    {
                        int currentProgress = (int)(pos/1024);
                        if (currentProgress <= maxProgress)
                        {
                            progressForm.setProgress(currentProgress);
                            Application.DoEvents();
                        }
                    }
                    lastLineStartPos = pos-1;
                    previousWasR = thisIsR;
                    thisIsR = false;
                    switch (c)
                    {
                        case    -1:
                            break;

                        case    'F':
                        case    'f':
                        {
                            c = ReadChar();
                            int funcIndex = ReadInt();
                            while (c == ' ' || c == '\t')
                                c = ReadChar();
                            sb.Length = 0;
                            while (c > ' ')
                            {
                                sb.Append((char)c);
                                c = ReadChar();
                            }
                            string name = sb.ToString();
                            while (c == ' ' || c == '\t')
                                c = ReadChar();
                            sb.Length = 0;
                            while (c > '\r')
                            {
                                sb.Append((char)c);
                                if (c == ')')
                                {
                                    c = ReadChar();
                                    break;
                                }
                                c = ReadChar();
                            }
                            string signature = sb.ToString();

                            uint addr = ReadUInt();
                            uint size = ReadUInt();
                            int modIndex = ReadInt();
                            int stackIndex = ReadInt();

                            if (c != -1)
                            {
                                EnsureStringCapacity(funcIndex, ref funcName);
                                funcName[funcIndex] = name;
                                EnsureStringCapacity(funcIndex, ref funcSignature);
                                funcSignature[funcIndex] = signature;
                                EnsureIntCapacity(funcIndex, ref funcModule);
                                funcModule[funcIndex] = modIndex;

                                if (stackIndex >= 0 && readLogResult.functionList != null)
                                    readLogResult.functionList.Add(funcIndex, stackIndex, size, modIndex);
                            }
                            break;
                        }

                        case    'T':
                        case    't':
                        {
                            c = ReadChar();
                            int typeIndex = ReadInt();
                            while (c == ' ' || c == '\t')
                                c = ReadChar();
                            if (c != -1 && Char.IsDigit((char)c))
                            {
                                if (ReadInt() != 0)
                                {
                                    finalizableTypes[typeIndex] = true;
                                }
                            }
                            while (c == ' ' || c == '\t')
                                c = ReadChar();
                            sb.Length = 0;
                            while (c > '\r')
                            {
                                sb.Append((char)c);
                                c = ReadChar();
                            }
                            string typeName = sb.ToString();
                            if (c != -1)
                            {
                                AddTypeName(typeIndex, typeName);
                            }
                            break;
                        }

                        // 'A' with thread identifier
                        case    '!':
                        {
                            c = ReadChar();
                            int threadId = ReadInt();
                            uint id = ReadUInt();
                            int typeSizeStackTraceIndex = ReadInt();
                            if (c != -1)
                            {
                                if (readLogResult.liveObjectTable != null)
                                    readLogResult.liveObjectTable.InsertObject(id, typeSizeStackTraceIndex, lastTickIndex, lastTickIndex, true, readLogResult.sampleObjectTable);
                                if (pos >= startFileOffset && pos < endFileOffset && readLogResult.allocatedHistogram != null)
                                {
                                    // readLogResult.calls.Add(new CallOrAlloc(false, threadId, typeSizeStackTraceIndex));
                                    readLogResult.allocatedHistogram.AddObject(typeSizeStackTraceIndex, 1);
                                }
                                ArrayList prev = (ArrayList)assembliesJustLoaded[threadId];
                                if(prev != null && prev.Count != 0)
                                {
                                    foreach(string assemblyName in prev)
                                    {
                                        assemblies[assemblyName] = -typeSizeStackTraceIndex;
                                    }
                                    prev.Clear();
                                }
                            }
                            readLogResult.hadAllocInfo = true;
                            readLogResult.hadCallInfo = true;
                            break;
                        }

                        case    'A':
                        case    'a':
                        {
                            c = ReadChar();
                            uint id = ReadUInt();
                            int typeSizeStackTraceIndex = ReadInt();
                            if (c != -1)
                            {
                                if (readLogResult.liveObjectTable != null)
                                    readLogResult.liveObjectTable.InsertObject(id, typeSizeStackTraceIndex, lastTickIndex, lastTickIndex, true, readLogResult.sampleObjectTable);
                                if (pos >= startFileOffset && pos < endFileOffset && readLogResult.allocatedHistogram != null)
                                {
                                    // readLogResult.calls.Add(new CallOrAlloc(false, typeSizeStackTraceIndex));
                                    readLogResult.allocatedHistogram.AddObject(typeSizeStackTraceIndex, 1);
                                }
                            }
                            readLogResult.hadAllocInfo = true;
                            readLogResult.hadCallInfo = true;
                            break;
                        }

                        case    'C':
                        case    'c':
                        {
                            c = ReadChar();
                            if (pos <  startFileOffset || pos >= endFileOffset)
                            {
                                while (c >= ' ')
                                    c = ReadChar();
                                break;
                            }
                            int threadIndex = ReadInt();
                            int stackTraceIndex = ReadInt();
                            if (c != -1)
                            {
                                if (readLogResult.callstackHistogram != null)
                                {
                                    readLogResult.callstackHistogram.AddObject(stackTraceIndex, 1);
                                }
                                ArrayList prev = (ArrayList)assembliesJustLoaded[threadIndex];
                                if(prev != null && prev.Count != 0)
                                {
                                    foreach(string assemblyName in prev)
                                    {
                                        assemblies[assemblyName] = stackTraceIndex;
                                    }
                                    prev.Clear();
                                }
                            }
                            readLogResult.hadCallInfo = true;
                            break;
                        }

                        case    'R':
                        case    'r':
                        {
                            c = ReadChar();
                            if (pos <  startFileOffset || pos >= endFileOffset)
                            {
                                while (c >= ' ')
                                    c = ReadChar();
                                break;
                            }
                            thisIsR = true;
                            if (!previousWasR)
                            {
                                if (readLogResult.objectGraph != null && readLogResult.objectGraph.idToObject.Count != 0)
                                    readLogResult.objectGraph.BuildTypeGraph();
                                readLogResult.objectGraph = new ObjectGraph(this, lastTickIndex);
                            }
                            stackPtr = 0;
                            uint objectID;
                            while ((objectID = ReadUInt()) != uint.MaxValue)
                            {
                                if (objectID > 0)
                                {
                                    uintStack[stackPtr] = objectID;
                                    stackPtr++;
                                    if (stackPtr >= uintStack.Length)
                                        uintStack = GrowUIntVector(uintStack);
                                }
                            }
                            if (c != -1)
                            {
                                if (readLogResult.objectGraph != null)
                                    readLogResult.objectGraph.roots = readLogResult.objectGraph.LookupReferences(stackPtr, uintStack);
                            }
                            break;
                        }

                        case    'O':
                        case    'o':
                        {
                            c = ReadChar();
                            if (pos <  startFileOffset || pos >= endFileOffset || readLogResult.objectGraph == null)
                            {
                                while (c >= ' ')
                                    c = ReadChar();
                                break;
                            }
                            uint objectId = ReadUInt();
                            int typeIndex = ReadInt();
                            uint size = ReadUInt();
                            stackPtr = 0;
                            uint objectID;
                            while ((objectID = ReadUInt()) != uint.MaxValue)
                            {
                                if (objectID > 0)
                                {
                                    uintStack[stackPtr] = objectID;
                                    stackPtr++;
                                    if (stackPtr >= uintStack.Length)
                                        uintStack = GrowUIntVector(uintStack);
                                }
                            }
                            if (c != -1)
                            {
                                ObjectGraph objectGraph = readLogResult.objectGraph;
                                ObjectGraph.GcType gcType = objectGraph.GetOrCreateGcType(typeName[typeIndex], typeIndex);
                                ObjectGraph.GcObject gcObject = objectGraph.GetOrCreateObject(objectId);
                                gcObject.size = size;
                                gcObject.type = gcType;
                                gcType.cumulativeSize += size;
                                gcObject.references = objectGraph.LookupReferences(stackPtr, uintStack);

                                // try to find the allocation stack trace and allocation time
                                // from the live object table
                                LiveObjectTable.LiveObject liveObject;
                                readLogResult.liveObjectTable.GetNextObject(objectId, objectId, out liveObject);
                                if (liveObject.id == objectId)
                                {
                                    gcObject.typeSizeStackTraceId = liveObject.typeSizeStacktraceIndex;
                                    gcObject.allocTickIndex = liveObject.allocTickIndex;
                                }
                            }
                            break;
                        }

                        case    'M':
                        case    'm':
                        {
                            c = ReadChar();
                            int modIndex = ReadInt();
                            sb.Length = 0;
                            while (c > '\r')
                            {
                                sb.Append((char)c);
                                c = ReadChar();
                            }
                            if (c != -1)
                            {
                                string lineString = sb.ToString();
                                int addrPos = lineString.LastIndexOf(" 0x");
                                if (addrPos <= 0)
                                    addrPos = lineString.Length;
                                int backSlashPos = lineString.LastIndexOf(@"\");
                                if (backSlashPos <= 0)
                                    backSlashPos = -1;
                                string basicName = lineString.Substring(backSlashPos + 1, addrPos - backSlashPos - 1);
                                string fullName = lineString.Substring(0, addrPos);

                                EnsureStringCapacity(modIndex, ref modBasicName);
                                modBasicName[modIndex] = basicName;
                                EnsureStringCapacity(modIndex, ref modFullName);
                                modFullName[modIndex] = fullName;
                            }
                            break;
                        }

                        case    'U':
                        case    'u':
                            c = ReadChar();
                            uint oldId = ReadUInt();
                            uint newId = ReadUInt();
                            uint length = ReadUInt();
                            Histogram reloHist = null;
                            if (pos >= startFileOffset && pos < endFileOffset)
                                reloHist = readLogResult.relocatedHistogram;
                            if (readLogResult.liveObjectTable != null)
                                readLogResult.liveObjectTable.UpdateObjects(reloHist, oldId, newId, length, lastTickIndex, readLogResult.sampleObjectTable);
                            break;

                        case    'I':
                        case    'i':
                            c = ReadChar();
                            lastTickIndex = AddTimePos(ReadInt(), lastLineStartPos);
                            break;

                        case    'G':
                        case    'g':
                            c = ReadChar();
                            int gcGen0Count = ReadInt();
                            int gcGen1Count = ReadInt();
                            int gcGen2Count = ReadInt();
                            if (readLogResult.liveObjectTable != null)
                            {
                                if (c == -1 || gcGen0Count < 0)
                                    readLogResult.liveObjectTable.RecordGc(lastTickIndex, 0, readLogResult.sampleObjectTable, gcGen0Count < 0);
                                else
                                    readLogResult.liveObjectTable.RecordGc(lastTickIndex, gcGen0Count, gcGen1Count, gcGen2Count, readLogResult.sampleObjectTable);
                            }
                            break;

                        case    'N':
                        case    'n':
                        {
                            c = ReadChar();
                            int funcIndex, stackTraceIndex = ReadInt();
                            stackPtr = 0;

                            int flag = ReadInt();
                            int matched = flag / 4;
                            int hadTypeId = (flag & 2);
                            bool hasTypeId = (flag & 1) == 1;

                            if (hasTypeId)
                            {
                                intStack[stackPtr++] = ReadInt();
                                intStack[stackPtr++] = ReadInt();
                            }

                            if (matched > 0)
                            {
                                /* use some other stack trace as a reference */
                                int otherStackTraceId = ReadInt();

                                int[] stacktrace = stacktraceTable.IndexToStacktrace(otherStackTraceId);
                                for(int i = 0; i < matched; i++)
                                {
                                    intStack[stackPtr++] = stacktrace[i + hadTypeId];
                                    if (stackPtr >= intStack.Length)
                                    {
                                        intStack = GrowIntVector(intStack);
                                    }
                                }
                            }

                            while ((funcIndex = ReadInt()) >= 0)
                            {
                                intStack[stackPtr] = funcIndex;
                                stackPtr++;
                                if (stackPtr >= intStack.Length)
                                    intStack = GrowIntVector(intStack);
                            }

                            if (c != -1)
                            {
                                stacktraceTable.Add(stackTraceIndex, intStack, stackPtr);
                            }
                            break;
                        }

                        case 'y':
                        case 'Y':
                        {
                            c = ReadChar();
                            int threadid = ReadInt();
                            if(!assembliesJustLoaded.Contains(threadid))
                            {
                                assembliesJustLoaded[threadid] = new ArrayList();
                            }
                            /* int assemblyId = */ ReadInt();

                            while (c == ' ' || c == '\t')
                            {
                                c = ReadChar();
                            }
                            sb.Length = 0;
                            while (c > '\r')
                            {
                                sb.Append((char)c);
                                c = ReadChar();
                            }
                            string assemblyName = sb.ToString();
                            ((ArrayList)assembliesJustLoaded[threadid]).Add(assemblyName);
                            break;
                        }

                        case 'S':
                        case 's':
                        {
                            c = ReadChar();
                            int stackTraceIndex = ReadInt();
                            int funcIndex;
                            stackPtr = 0;
                            while ((funcIndex = ReadInt()) >= 0)
                            {
                                intStack[stackPtr] = funcIndex;
                                stackPtr++;
                                if (stackPtr >= intStack.Length)
                                    intStack = GrowIntVector(intStack);
                            }
                            if (c != -1)
                            {
                                stacktraceTable.Add(stackTraceIndex, intStack, stackPtr);
                            }
                            break;
                        }

                        case    'Z':
                        case    'z':
                        {
                            sb.Length = 0;
                            c = ReadChar();
                            while (c == ' ' || c == '\t')
                                c = ReadChar();
                            while (c > '\r')
                            {
                                sb.Append((char)c);
                                c = ReadChar();
                            }
                            string commentString = sb.ToString();
                            if (c != -1)
                            {
                                if (commentTable == null)
                                    commentTable = new string[5];
                                EnsureStringCapacity(commentIndex, ref commentTable);
                                commentTable[commentIndex] = commentString;

                                if (readLogResult.sampleObjectTable != null)
                                    readLogResult.sampleObjectTable.RecordComment(lastTickIndex, commentIndex);

                                commentIndex++;
                            }
                            break;
                        }

                        default:
                        {
                            // just ignore the unknown
                            // just ignore the unknown
                            while(c != '\n' && c != '\r')
                            {
                                c = ReadChar();
                            }
                            break;
                        }
                    }
                    while (c == ' ' || c == '\t')
                        c = ReadChar();
                    if (c == '\r')
                        c = ReadChar();
                    if (c == '\n')
                    {
                        c = ReadChar();
                        line++;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Bad format in log file {0} line {1}", fileName, line));
            }

            finally
            {
                progressForm.Visible = false;
                progressForm.Dispose();
                if (r != null)
                    r.Close();
            }
        }
    }
}
