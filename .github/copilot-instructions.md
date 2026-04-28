# Copilot instructions — SerialCommunication

Purpose: concise guidance to help Copilot sessions understand and work in this repository.

## Quick commands
- Build (Windows / CLI):
  - msbuild "SerialCommunication.slnx" /p:Configuration=Debug
  - or: msbuild "SerialCommunication\SerialCommunication.csproj" /p:Configuration=Debug
- Run (after build): bin\Debug\SerialCommunication.exe (Windows)
- Arduino (sketch at repository root):
  - Compile: arduino-cli compile --fqbn <fqbn> "./"
  - Upload: arduino-cli upload -p <PORT> --fqbn <fqbn> "./"
  - Replace <fqbn> and <PORT> with board FQBN and serial port. Arduino IDE also works.
- Tests / linters: none present in this repo (no unit test project or linter configuration). Single-test command: N/A.

## High-level architecture
- Two primary components:
  1. Desktop GUI (C# / WinForms)
     - Location: SerialCommunication/ (Visual Studio project: SerialCommunication.csproj)
     - Target: .NET Framework 4.7.2, WinExe produced in bin\(Debug|Release)
     - Entry point: Program.Main -> Application.Run(new Form1())
     - Purpose: enumerate serial ports, select baudrate (defaults to 115200), and interact with a microcontroller over a serial link.
     - Key API: System.IO.Ports.SerialPort used to communicate.
  2. Embedded sketch / Arduino
     - Files: SerialCommunication.ino (root), SerialCommand.cpp/.h, analog.c
     - Purpose: Run on an AVR/Arduino-style MCU and accept simple ASCII serial commands (set, toggle, get, ping, help, debug).
     - Protocol: ASCII commands tokenized by SerialCommand library; commands terminated by newline/CR. Example commands:
       - "set d2 on\n"
       - "set pwm9 128\n"
       - "get a0\n"
       - "ping\n"

- Interaction: desktop app opens serial port at chosen baud (115200 recommended) and exchanges ASCII commands with the MCU.

## Key repository conventions and patterns
- Project is Windows-first: use Visual Studio/MSBuild. Avoid dotnet CLI (this is .NET Framework, not .NET Core).
- Serial settings: Arduino sketch expects 115200 by default (Baudrate define in .ino). Desktop app sets default comboBoxBaudrate to "115200".
- Arduino SerialCommand usage:
  - Commands are registered in setup() via sCmd.addCommand("name", handler);
  - Default handler set with sCmd.setDefaultHandler(onUnknownCommand);
  - Buffer sizes and limits are configured in SerialCommand.h (SERIALCOMMANDBUFFER, MAXSERIALCOMMANDS).
  - To enable verbose debug echoing, define SERIALCOMMANDDEBUG in SerialCommand.h (currently undefined).
- Pin mapping expectations (as used by the sketch):
  - Digital outputs: D2..D4
  - PWM outputs: D9..D11
  - Digital inputs: D5..D7
  - Analog inputs: A0..A5
- Do not commit build artifacts: .gitignore follows standard Visual Studio template (bin/, obj/, .vs/, etc.).
- To extend commands: modify SerialCommunication.ino and the SerialCommand callbacks; keep tokenization rules (space-delimited) in mind.

## Existing docs and assistant configs
- No README.md, CONTRIBUTING.md, or AI assistant rule files (CLAUDE.md, .cursorrules, AGENTS.md, etc.) were found. This file should be the primary Copilot guide.

## Notes for Copilot sessions
- When suggesting code changes for the desktop app, prefer MSBuild/Visual Studio-compatible edits and preserve WinForms Designer-generated files (Form1.Designer.cs).
- When suggesting Arduino changes, follow existing SerialCommand API and keep message terminator and buffer-size constraints in mind.

---

(If you want more: include upload examples with specific board FQBNs, or add explicit troubleshooting steps for common COM/driver issues.)
