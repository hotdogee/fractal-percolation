# ProfilerOBJ makefile
# Copyright (c) Microsoft Corporation.  All rights reserved.

OBJECTS=ProfilerCallback.obj \
	ProfilerInfo.obj

DEFINES = /DWIN32 /DNDEBUG /D_WINDOWS /D_USRDLL /DPROFILEROBJ_EXPORTS /D_SAMPLES_

SWITCHES = /EHsc

LIBS = corguids.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib winmm.lib

LINKEROPTIONS = /OUT:profilerOBJ.dll /DLL /DEF:profilerOBJ.def

profilerOBJ.dll : $(OBJECTS)
	link $(OBJECTS) $(LIBS) $(LINKEROPTIONS)

{$S}.cpp{$O}.obj:
	$(CC) $(DEFINES) $(SWITCHES) -c $<

clean:
	del profilerOBJ.dll profilerOBJ.lib profilerOBJ.exp $(OBJECTS)
