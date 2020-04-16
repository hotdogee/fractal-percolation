# CLRProfilerControl makefile
# Copyright (c) Microsoft Corporation.  All rights reserved.

SOURCES=CLRProfilerControl.cs

CLRProfilerControl.dll : $(SOURCES)
	csc /t:library /out:CLRProfilerControl.dll $(SOURCES)

clean:
	del CLRProfilerControl.dll
