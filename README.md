# fractal-percolation

Percolation probability exploration in randomly generated fractal maps

## Languages and Libraries

* C#
* Windows.Forms
* fftw

# Spectrally synthesized fractal motion in two dimensions

This project implements the algorithm given under the name SpectralSynthesisFM2D on page 108 of Peitgen & Saupe.
The method uses Fractional Brownian Motion (fBM) noise to generate a matrix of phase and amplitude data (complex numbers) in the frequency domain associated with Fourier methodology, then perform an inverse Fast Fourier Transform (FFT) to create a heightfield in the time (or space) domain of the Euclidean world. The transformed matrix is rendered after conversion to an array of polygons. This method is sometimes called Spectral Synthesis. Spectral Synthesis provides a much more realistic landscape but is MUCH more computationally intensive.

# Features

* Map Generation
  * Fixed Count
  * Random Count
  * Fractal
    * Supports negative Hurst exponent
* Map Visualization
  * Customizable Cell Size, Border Size, Passable Color, Obstacle Color, Border Color, Path Color
  * Option to Hide Path and Patterned Path
* Path Search
  * Implemented algorithms
    * Priority Queue
    * Depth First Search
    * Sorted Set
  * Direction
    * 4 Way
    * 8 Way
  * Jump
* Multi-threaded task loop manager

# Screenshots

![Path 0.8](screenshots/Path0.8.jpg?raw=true "Path 0.8")

# ChangeLog

0.2
- Option to set border width and cell size

0.3
- New spectral synthesis fractal Brownian motion map generation algorithm

0.3a
- Remembers window size

0.3b
- Fixed non square fractal maps

0.8
- Fixed Current Window not working after closing a FormDisplay window
- Added Output Window
- Use FFTW library instead of Dew.Math, run times are 2 times faster
- Disable threading when using fractal fill algorithm
- Abort unfinished threads when closing the window

0.8a
- Fixed "Fixed Count" map gen algorithm

0.8b
- Code clean up
- Tweaked FormControl controls to look better in 120dpi

0.8c
- Removed "%" from FormSearchResults output

0.8d
- Output modifications

0.8e
- Recompiled fftwlib to support multithreading
- Fixed crashes when running more then one fractal search
- Reenabled threading in fractal fill algorithm

0.8f
- Output file name changed to .csv
- Output file string reorganized
- FormSearchResults added estimated run time