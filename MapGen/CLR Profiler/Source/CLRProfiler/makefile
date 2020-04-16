# CLRProfiler makefile
# Copyright (c) Microsoft Corporation.  All rights reserved.

SOURCES=AgeHistogram.cs \
	AssemblyInfo.cs \
	BitReader.cs \
	BitWriter.cs \
	CallTreeForm.cs \
	CommentRangeForm.cs \
	Edge.cs \
	FilterForm.cs \
	FunctionFilter.cs \
	FunctionFind.cs \
	Form1.cs \
	Form2.cs \
	Form3.cs \
	Form5.cs \
	Graph.cs \
	GraphViewForm.cs \
	HistogramViewForm.cs \
	KillProcessForm.cs \
	ListViewer.cs \
	NamedManualResetEvent.cs \
	ObjectGraph.cs \
	PlacedToolTip.cs \
	ProfileServiceForm.cs \
	Readers.cs \
	ReadNewLog.cs \
	RegisterDLL.cs \
	SaveFileForm.cs \
	SelectColumns.cs \
	SortAndHighlightSelector.cs \
	TimeLineViewForm.cs \
	TreeListView.cs \
	Vertex.cs \
	ViewByAddressForm.cs \
	ViewCommentsForm.cs \
	ViewFilter.cs \
	WaitingForConnectionForm.cs

CLRProfiler.exe : $(SOURCES)
	csc /t:winexe /out:CLRProfiler.exe /unsafe /r:System.Windows.Forms.dll $(SOURCES)

clean:
	del CLRProfiler.exe
