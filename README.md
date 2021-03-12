# Introduction 

The goal of this side project is to use an EPaper display to display custom messages or the weather. A full Report can be found [here](https://gf3r.visualstudio.com/EPaper/_git/EPaper.Web?path=%2Fdocs%2FSummary.md&version=GBmain&_a=preview).

## Components

The components used were the following:

- [EPaper display](https://www.bastelgarage.ch/400x300-4-2inch-e-ink-display?search=400x300%204.2inch%20E-Ink%20Display)
- [ESP32](https://www.bastelgarage.ch/wemos-lolin32-lite-board-esp32-rev1-4-mb-flash?search=esp32)
- Raspberry Pi
- Cables

## Code

The code is split up into 4 areas:

- [backend](./backend) - The ASP .NET core application, responsible for handling and sending image data.
- [frontend](./frontend) - The angular application used to debug the backend.
- [embedded](./embedded) - The C++ code for the ESP32, visualising the image data.
- [build](./build) - WIP