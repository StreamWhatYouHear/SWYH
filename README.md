# Stream What You Hear

<p align="center">
  <img src="SWYH/Resources/Icons/swyh.svg" alt="Stream What You Hear" width="180">
</p>

<p align="center">
  <strong>Stream your PC audio to UPnP/DLNA-compatible devices.</strong>
</p>

<p align="center">
  <a href="https://github.com/StreamWhatYouHear/SWYH/releases">
    <img src="https://img.shields.io/github/v/release/StreamWhatYouHear/SWYH?include_prereleases&label=release" alt="Latest release">
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/github/license/StreamWhatYouHear/SWYH" alt="License">
  </a>
  <img src="https://img.shields.io/badge/.NET-10-512BD4" alt=".NET 10">
</p>

**Stream What You Hear (SWYH)** is a lightweight Windows application that captures the audio playing on your PC and streams it to compatible TVs, amplifiers, network speakers, media players and other UPnP/DLNA devices.

Free, open source, and built for Windows.

**[Download SWYH](https://www.streamwhatyouhear.com/download/)** ·
**[Getting Started](https://www.streamwhatyouhear.com/getting-started/)** ·
**[Website](https://www.streamwhatyouhear.com/)**


## SWYH 2

SWYH 2 is a major modernization of the project.

More than six years after the previous major release, SWYH has been rebuilt on a modern foundation:

- Migrated from **.NET Framework 4.0 to .NET 10**
- Completely redesigned Windows interface
- New SWYH visual identity
- Major architecture and codebase refactoring
- Modern audio capture and streaming pipeline
- GitHub Actions CI/CD and automated releases
- Renewed open-source development


## Features

- **Capture system audio**  
  Stream any audio playing on your Windows PC.

- **Automatic UPnP/DLNA discovery**  
  Discover compatible Media Renderers on your local network and start playback directly from SWYH.

- **HTTP audio streams**  
  Use the built-in stream URLs with compatible players and devices.

- **MP3 and PCM/L16 streaming**  
  Select the format best suited to your playback device.

- **Record What You Hear**  
  Save the audio playing on your PC directly to an MP3 file, with live level monitoring and recording duration.

- **System integration**  
  Options include local audio muting, sleep prevention and process priority control.

## Getting Started

1. **Download SWYH**

   Download the latest release from  
   https://www.streamwhatyouhear.com/download/

   SWYH is available as:

   - **Setup** — recommended for a standard Windows installation
   - **Portable** — extract and run SWYH without installing it

2. **Choose your audio source**

   Select the Windows audio device you want SWYH to capture.

3. **Choose a renderer**

   SWYH automatically discovers compatible UPnP/DLNA Media Renderers on your local network.

4. **Press Play**

   Select a device and start streaming.

That's it — audio playing on your PC is now streamed to your renderer.

For more details, see the **[Getting Started guide](https://www.streamwhatyouhear.com/getting-started/)**.

## Media Renderer or Media Player?

UPnP/DLNA devices do not all behave in the same way.

### Media Renderer

A Media Renderer can be discovered and remotely controlled by SWYH.

Select the device in SWYH and press **Play**.

### Media Player

A Media Player can consume the SWYH stream, but playback must be started from the device itself.

You can copy one of the HTTP Stream URLs exposed by SWYH and open it manually on the player.

## Compatibility

SWYH uses standard UPnP/DLNA protocols and works with many compatible:

- TVs
- AV receivers and amplifiers
- Network speakers
- Media players
- Software renderers

Compatibility depends on the UPnP/DLNA implementation and supported audio formats of the receiving device.

If your device does not appear in SWYH, check the
**[FAQ](https://www.streamwhatyouhear.com/faq/)** or
**[open an issue](https://github.com/StreamWhatYouHear/SWYH/issues)**.

## Download

Latest versions are available from:

**https://github.com/StreamWhatYouHear/SWYH/releases**

SWYH 2 releases are provided as both **Setup** and **Portable** packages when available.

## A small project with a long history

SWYH started in **2012** as a personal project by [Sébastien Warin](https://sebastien.warin.fr) to solve a simple problem: send the audio playing on a Windows PC to a network audio device.

- **2012** — Created and first released
- **2015** — Released as open source
- **2020** — SWYH 1.5
- **2026** — SWYH 2.0, rebuilt on .NET 10

Over its lifetime, SWYH has been downloaded **nearly 2 million times**.

The original SWYH announcement from October 2012 is still available:

**[Read the original article](https://sebastien.warin.fr/2012/10/12/1186-swyh-stream-what-you-hear-envoyer-le-son-de-votre-pc-windows-sur-un-lecteur-upnp-dlna/)** *(French)*

You can also watch the original 2012 demonstration:

**[Watch the original SWYH video](https://www.youtube.com/watch?v=AxGB7roh4-c)**

## Technology

SWYH 2 is built with:

- **.NET 10**
- **WASAPI** for Windows audio capture
- **NAudio** for audio processing
- **LAME** for MP3 encoding
- **UPnP/DLNA** for device discovery and remote playback
- Adapted Intel UPnP/DLNA AV components for renderer and ContentDirectory support

Third-party component details are available in
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## Contributing

Contributions, bug reports and feedback are welcome.

See [CONTRIBUTING.md](CONTRIBUTING.md) for information about contributing to SWYH.

If you find a bug, please include:

- SWYH version
- Windows version
- playback device / renderer
- steps to reproduce the issue

[Open an issue](https://github.com/StreamWhatYouHear/SWYH/issues)

## License

Stream What You Hear is released under the **GNU GPL** license.

See [LICENSE](LICENSE) for full details.