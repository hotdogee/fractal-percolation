# CLRProfiler makefile
# Copyright (c) Microsoft Corporation. All rights reserved.

all:
	cd CLRProfiler
	nmake
	cd ..
	
	cd ProfilerOBJ
	nmake
	cd ..

	cd CLRProfilerControl
	nmake
	cd ..

	copy CLRProfiler\CLRProfiler.exe
	copy ProfilerOBJ\ProfilerObj.dll
	copy CLRProfilerControl\ClrProfilerControl.dll

clean:
	del CLRProfiler.exe
	del ProfilerOBJ.dll
	del CLRProfilerControl.dll

	cd CLRProfiler
	nmake clean
	cd ..
	
	cd ProfilerOBJ
	nmake clean
	cd ..

	cd CLRProfilerControl
	nmake clean
	cd ..
