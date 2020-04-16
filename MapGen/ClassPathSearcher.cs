using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MapGen
{
    public class PathSearcherInput
    {
        #region Input variables
        private int _searchAlgorithm; // PriorityQueue = 1, DepthFirst = 2
        private int _searchType; // 4way = 1, 8way = 2
        private int _jump;
        #endregion
        
        #region Properties
        public int SearchAlgorithm
        {
            get { return _searchAlgorithm; }
            set { _searchAlgorithm = value; }
        }
        public int SearchType
        {
            get { return _searchType; }
            set { _searchType = value; }
        }
        public int Jump
        {
            get { return _jump; }
            set { _jump = value >= 0 ? value : 0; }
        }
        #endregion

        #region Constructor
        public PathSearcherInput() : this(1, 1, 0)
        {
        }
        public PathSearcherInput(int searchAlgorithm) : this(searchAlgorithm, 1, 0)
        {
        }
        public PathSearcherInput(int searchAlgorithm, int searchType) : this(searchAlgorithm, searchType, 0)
        {
        }
        public PathSearcherInput(int searchAlgorithm, int searchType, int jump)
        {
            SearchAlgorithm = searchAlgorithm;
            SearchType = searchType;
            Jump = jump;
        }
        #endregion
    }

    public class PathSearcher
    {
        private byte[][] _byteMap;
        private Node[][] _nodeMap;
        private MapGeneratorInput _mapInput;
        private PathSearcherInput _pathInput;
        private MapGenerator _mapGen;

        #region Properties
        public MapGeneratorInput MapInput
        {
            get {return _mapInput;}
        }
        public PathSearcherInput PathInput
        {
            get {return _pathInput;}
        }
        public Node[][] NodeMap
        {
            get { return _nodeMap; }
        }
        public byte[][] ByteMap
        {
            get { return _byteMap; }
        }
        #endregion

        #region Constructor
        public PathSearcher()
        {
            _mapInput = new MapGeneratorInput();
            _pathInput = new PathSearcherInput();
            _mapGen = new MapGenerator(_mapInput);
        }
        public PathSearcher(PathSearcherInput pathInput, MapGeneratorInput mapInput)
        {
            _mapInput = mapInput;
            _pathInput = pathInput;
            _mapGen = new MapGenerator(_mapInput);
        }
        #endregion

        #region Destructor
        ~PathSearcher()
        {
            _byteMap = null;
            _nodeMap = null;
            _mapInput = null;
            _pathInput = null;
            _mapGen = null;
        }
        #endregion

        // find path from top to down in I axis
        public bool RunOne(byte[][] byteMap)
        {
            // Generate a random map
            _byteMap = byteMap;
            //debug
            //_byteMap = MapGenerator.ReadMap(@"C:\Users\TPLIN\Desktop\Path_0.9_Source_64bit\bin\Debug\debug_map.xml");
            // Build node map
            //_nodeMap = new Node[_mapInput.Height, _mapInput.Width];
            _nodeMap = new Node[_mapInput.Height][];
            for (int i = 0; i < _mapInput.Height; i++)
            {
                _nodeMap[i] = new Node[_mapInput.Width];
                for (int j = 0; j < _mapInput.Width; j++)
                {
                    _nodeMap[i][j] = new Node();
                    _nodeMap[i][j].I = i;
                    _nodeMap[i][j].J = j;
                    if (_byteMap[j][i] == 0)
                    {
                        _nodeMap[i][j].Value = true;
                        // Mark end node
                        if (i >= _mapInput.Height - 1 - _pathInput.Jump)
                        {
                            _nodeMap[i][j].EndNode = true;
                        }
                    }
                }
            }
            // Loop through all possible starting nodes
            for (int i = 0; i <= _pathInput.Jump; i++)
                for (int j = 0; j < _mapInput.Width; j++)
                    if (_nodeMap[i][j].Value && !_nodeMap[i][j].Used) // Check if node is set and not used
                    {
                        // PriorityQueue = 1, DepthFirst = 2, SortedSet = 3
                        if (_pathInput.SearchAlgorithm == 1)
                        {
                            //if (PriorityQueueSearch_SortedSet(_nodeMap[i][j]))
                            if (PriorityQueueSearch(_nodeMap[i][j]))
                                return true;
                        }
                        else if (_pathInput.SearchAlgorithm == 2)
                        {
                            if (DepthFirstSearch(_nodeMap[i][j]))
                                return true;
                        }
                        else if (_pathInput.SearchAlgorithm == 3)
                        {
                            if (PriorityQueueSearch_SortedSet(_nodeMap[i][j]))
                                return true;
                        }
                    }
            return false;
        }

        // find path from top to down in I axis
        public bool RunOne()
        {
            // Generate a random map
            return RunOne(_mapGen.Generate());
        }

        private bool DepthFirstSearch(Node currentNode)
        {
            // Mark used
            currentNode.Used = true;
            // Check if this is an end node
            if (currentNode.EndNode)
            {
                return true;
            }

            int absI, absJ;

            for (int i = _pathInput.Jump + 1; i >= -(_pathInput.Jump + 1); i--) // relative I axis search range, from +max range to -max range
            {
                absI = currentNode.I + i; // calculate absolute I position
                if (absI >= 0 && absI < _mapInput.Height) // border check
                {
                    absJ = currentNode.J;
                    if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                        if (DepthFirstSearch(_nodeMap[absI][absJ])) // do j = 0
                            return true;
                    
                    int jJump = 0;
                    if (_pathInput.SearchType == 1) // 4way
                        jJump = _pathInput.Jump + 1 - Math.Abs(i);
                    else if (_pathInput.SearchType == 2) // 8way
                        jJump = _pathInput.Jump + 1;

                    for (int j = 1; j <= jJump; j++)
                    {
                        absJ = currentNode.J + j;
                        if (absJ < _mapInput.Width) // border check
                            if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                if (DepthFirstSearch(_nodeMap[absI][absJ])) // do +j 
                                    return true;
                        absJ = currentNode.J - j;
                        if (absJ >= 0) // border check
                            if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                if (DepthFirstSearch(_nodeMap[absI][absJ])) // do -j
                                    return true;
                    }
                }
            }
            return false;
        }

        private bool PriorityQueueSearch_SortedSet(Node startNode)
        {
            int absI, absJ;
            SortedSet<Node> queue = new SortedSet<Node>(new NodeComparer());

            startNode.Used = true;
            queue.Add(startNode);

            while (queue.Count > 0)
            {
                Node currentNode = queue.Max;
                queue.Remove(queue.Max);
                if (currentNode.EndNode)
                {
                    // color path
                    _byteMap[currentNode.J][currentNode.I] = 3;
                    do
                    {
                        currentNode = currentNode.Parent;
                        _byteMap[currentNode.J][currentNode.I] = 3;
                    } while (currentNode.Parent != null);
                    return true;
                }
                for (int i = _pathInput.Jump + 1; i >= -(_pathInput.Jump + 1); i--) // relative I axis search range, from +max range to -max range
                {
                    absI = currentNode.I + i; // calculate absolute I position
                    if (absI >= 0 && absI < _mapInput.Height) // border check
                    {
                        absJ = currentNode.J; // do j = 0 first
                        if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                        {
                            _nodeMap[absI][absJ].Used = true;
                            _nodeMap[absI][absJ].Parent = currentNode;
                            queue.Add(_nodeMap[absI][absJ]);
                        }

                        int jJump = 0;
                        if (_pathInput.SearchType == 1) // 4way
                            jJump = _pathInput.Jump + 1 - Math.Abs(i);
                        else if (_pathInput.SearchType == 2) // 8way
                            jJump = _pathInput.Jump + 1;

                        for (int j = 1; j <= jJump; j++)
                        {
                            absJ = currentNode.J + j; // do +j
                            if (absJ < _mapInput.Width) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Add(_nodeMap[absI][absJ]);
                                }
                            }
                            absJ = currentNode.J - j; // do -j
                            if (absJ >= 0) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Add(_nodeMap[absI][absJ]);
                                }
                            }
                        }
                    }
                }
            }
            queue = null;
            return false;
        }
        /*
        private bool PriorityQueueSearch(Node startNode)
        {
            int absI, absJ;
            BinaryHeapPriorityQueue queue = new BinaryHeapPriorityQueue(new IaxisComparer(), (int)(_mapInput.Height * 3));

            startNode.Used = true;
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                Node currentNode = (Node)queue.Dequeue();
                if (currentNode.EndNode)
                {
                    // color path
                    _byteMap[currentNode.J][currentNode.I] = 3;
                    do
                    {
                        currentNode = currentNode.Parent;
                        _byteMap[currentNode.J][currentNode.I] = 3;
                    } while (currentNode.Parent != null);
                    return true;
                }
                for (int i = _pathInput.Jump + 1; i >= -(_pathInput.Jump + 1); i--) // relative I axis search range, from +max range to -max range
                {
                    absI = currentNode.I + i; // calculate absolute I position
                    if (absI >= 0 && absI < _mapInput.Height) // border check
                    {
                        absJ = currentNode.J; // do j = 0 first
                        if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                        {
                            _nodeMap[absI][absJ].Used = true;
                            _nodeMap[absI][absJ].Parent = currentNode;
                            // is center node?
                            if (absI == 0 || absJ == 0 || absI ==
                            queue.Enqueue(_nodeMap[absI][absJ]);
                        }

                        int jJump = 0;
                        if (_pathInput.SearchType == 1) // 4way
                            jJump = _pathInput.Jump + 1 - Math.Abs(i);
                        else if (_pathInput.SearchType == 2) // 8way
                            jJump = _pathInput.Jump + 1;

                        for (int j = 1; j <= jJump; j++)
                        {
                            absJ = currentNode.J + j; // do +j
                            if (absJ < _mapInput.Width) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Enqueue(_nodeMap[absI][absJ]);
                                }
                            }
                            absJ = currentNode.J - j; // do -j
                            if (absJ >= 0) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Enqueue(_nodeMap[absI][absJ]);
                                }
                            }
                        }
                    }
                }
            }
            queue = null;
            return false;
        }*/

        private bool PriorityQueueSearch(Node startNode)
        {
            int absI, absJ;
            BinaryHeapPriorityQueue queue = new BinaryHeapPriorityQueue(new IaxisComparer(), (int)(_mapInput.Height * 3));

            startNode.Used = true;
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                Node currentNode = (Node)queue.Dequeue();
                if (currentNode.EndNode)
                {
                    // color path
                    _byteMap[currentNode.J][currentNode.I] = 3;
                    do
                    {
                        currentNode = currentNode.Parent;
                        _byteMap[currentNode.J][currentNode.I] = 3;
                    } while (currentNode.Parent != null);
                    return true;
                }
                for (int i = _pathInput.Jump + 1; i >= -(_pathInput.Jump + 1); i--) // relative I axis search range, from +max range to -max range
                {
                    absI = currentNode.I + i; // calculate absolute I position
                    if (absI >= 0 && absI < _mapInput.Height) // border check
                    {
                        absJ = currentNode.J; // do j = 0 first
                        if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                        {
                            _nodeMap[absI][absJ].Used = true;
                            _nodeMap[absI][absJ].Parent = currentNode;
                            queue.Enqueue(_nodeMap[absI][absJ]);
                        }
                        
                        int jJump = 0;
                        if (_pathInput.SearchType == 1) // 4way
                            jJump = _pathInput.Jump + 1 - Math.Abs(i);
                        else if (_pathInput.SearchType == 2) // 8way
                            jJump = _pathInput.Jump + 1;

                        for (int j = 1; j <= jJump; j++)
                        {
                            absJ = currentNode.J + j; // do +j
                            if (absJ < _mapInput.Width) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Enqueue(_nodeMap[absI][absJ]);
                                }
                            }
                            absJ = currentNode.J - j; // do -j
                            if (absJ >= 0) // border check
                            {
                                if (_nodeMap[absI][absJ].Value && !_nodeMap[absI][absJ].Used)
                                {
                                    _nodeMap[absI][absJ].Used = true;
                                    _nodeMap[absI][absJ].Parent = currentNode;
                                    queue.Enqueue(_nodeMap[absI][absJ]);
                                }
                            }
                        }
                    }
                }
            }
            queue = null;
            return false;
        }

        /*
        public void Print_nodeMap()
        {
            for (int i = 0; i < _mapInput.Height; i++)
            {
                for (int j = 0; j < _mapInput.Width; j++)
                {
                    if (_nodeMap[i, j].Value)
                        Console.Write("1 ");
                    else
                        Console.Write("0 ");
                }
                Console.Write("\n");
            }
        }
        */
    }

    public class Node
    {
        public bool Value = false; // 這個Node的值
        public bool Used = false; // 有沒有被走過
        public bool EndNode = false; // 是否為結束Node

        public Node Parent = null;

        public int I = 0;
        public int J = 0;
    }

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node a, Node b)
        {
            return Comparer<int>.Default.Compare(a.I * 8192 + a.J, b.I * 8192 + b.J);
        }
    } 

    public class IaxisComparer: IComparer
    {
        public int Compare(object a, object b)
        {
            return Comparer.Default.Compare(((Node)a).I, ((Node)b).I);
        } 
    } 

    public class JaxisComparer: IComparer
    {
        public int Compare(object a, object b)
        {
            return Comparer.Default.Compare(((Node)a).I, ((Node)b).I);
        } 
    } 

    public class BinaryHeapPriorityQueue : ICollection
    {
        protected ArrayList InnerList = new ArrayList();
        protected IComparer Comparer;

        #region contructors
        public BinaryHeapPriorityQueue() : this(System.Collections.Comparer.Default)
        {}
        public BinaryHeapPriorityQueue(IComparer c)
        {
            Comparer = c;
        }
        public BinaryHeapPriorityQueue(int C) : this(System.Collections.Comparer.Default,C)
        {}
        public BinaryHeapPriorityQueue(IComparer c, int Capacity)
        {
            Comparer = c;
            InnerList.Capacity = Capacity; // capacity grows automatically
        }

        protected BinaryHeapPriorityQueue(ArrayList Core, IComparer Comp, bool Copy)
        {
            if(Copy)
                InnerList = Core.Clone() as ArrayList;
            else
                InnerList = Core;
            Comparer = Comp;
        }

        #endregion
        protected void SwitchElements(int i, int j)
        {
            object h = InnerList[i];
            InnerList[i] = InnerList[j];
            InnerList[j] = h;
        }

        protected virtual int OnCompare(int i, int j)
        {
            return Comparer.Compare(InnerList[i], InnerList[j]);
        }

        #region public methods
        public int Enqueue(object target)
        {
            int index = InnerList.Count;
            int parent = (index - 1) / 2;;
            InnerList.Add(target); // added to InnerList[index]
            // max heap
            while ((index > 0) && (Comparer.Compare(target, InnerList[parent]) > 0)) // if target > parent
            {
                InnerList[index] = InnerList[parent]; // parent moves down
                index = parent;
                parent = (index - 1) / 2;
            }
            InnerList[index] = target;
            return index;
        }

        public object Dequeue()
        {
            if (InnerList.Count == 0)
                return null;
            int target = 0, child = 1;
            object result = InnerList[0];
            object lastEntry = InnerList[InnerList.Count - 1]; // save last entry
            InnerList.RemoveAt(InnerList.Count - 1); // remove last entry
            if (InnerList.Count == 0)
                return result;
            while (child < InnerList.Count)
            {
                // if right child exists, pick the bigger one
                if (((child + 1) < InnerList.Count) && (Comparer.Compare(InnerList[child], InnerList[child + 1]) < 0))
                    child = child + 1;
                // pick left child if bigger then lastEntry
                if (Comparer.Compare(lastEntry, InnerList[child]) < 0)
                {
                    InnerList[target] = InnerList[child];
                    target = child;
                    child = 2 * target + 1;
                }
                else // lastEntry is bigger then both childs of target
                {
                    break;
                }
            }
            InnerList[target] = lastEntry;
            return result;
        }
        
        public object Peek()
        {
            if(InnerList.Count > 0)
                return InnerList[0];
            return null;
        }

        public bool Contains(object value)
        {
            return InnerList.Contains(value);
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public int Count
        {
            get
            {
                return InnerList.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            InnerList.CopyTo(array,index);
        }

        public object Clone()
        {
            return new BinaryHeapPriorityQueue(InnerList, Comparer, true);	
        }

        public bool IsSynchronized
        {
            get
            {
                return InnerList.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }
        #endregion
    }
}
