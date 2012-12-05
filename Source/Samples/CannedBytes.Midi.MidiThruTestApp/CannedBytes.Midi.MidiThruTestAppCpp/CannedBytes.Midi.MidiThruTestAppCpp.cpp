// CannedBytes.Midi.MidiThruTestAppCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#pragma comment(lib, "winmm.lib")

HMIDIOUT hMidiOut = NULL;
HMIDIIN hMidiIn = NULL;

void CALLBACK MidiInProc(HMIDIIN hMidiIn, UINT msg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2)
{
	if (msg == MIM_DATA)
	{
		System::Diagnostics::Stopwatch^ sw = gcnew System::Diagnostics::Stopwatch();
        sw->Start();

        ::midiOutShortMsg(hMidiOut, dwParam1);

        sw->Stop();
		float milleSecs = ((float)sw->ElapsedTicks / (float)System::Diagnostics::Stopwatch::Frequency) * 1000;
        
		std::cout << "Midi Out ms: " << milleSecs << std::endl;
	}
}

void PrintUsage()
{
	std::cout << "CannedBytes.Midi.MidiThruTestAppCpp InPortId OutPortId" << std::endl;
}

int _tmain(int argc, _TCHAR* argv[])
{
	int inPortId = 0;
	int outPortId = 0;

	// argv[0] is executable name
	if (argc == 3)
	{
		inPortId = _wtoi(argv[1]);
		inPortId = _wtoi(argv[2]);
	}
	else if (argc != 1)
	{
		PrintUsage();
		return -1;
	}

	
	int result = ::midiOutOpen(&hMidiOut, outPortId, NULL, NULL, 0);

	if (result != 0)
	{
		std::cout << "Midi Out Port could not be opened." << std::endl;
		return -1;
	}

	result = ::midiInOpen(&hMidiIn, inPortId, (DWORD_PTR)&MidiInProc, 0, CALLBACK_FUNCTION);

	if (result != 0)
	{
		std::cout << "Midi In Port could not be opened." << std::endl;
		return -1;
	}

	::midiInStart(hMidiIn);

	std::cout << "Press any key to exit..." << std::endl;
	getchar();

	return 0;
}


