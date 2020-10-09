# FileSearch
Easy (binary) search for strings in files. Uses KMP matching algorithm and different encodings.

Expects a string and a directory/file path as arguments. Third argument is filename filter (optional, GetFiles()-style, i.e. regex not supported).

Generates byte patterns for all available (ASCII, UTF-16 LE and BE, UTF-7, UTF-8, UTF-32) encodings, chooses distinct ones and looks for them using Knuth-Morris-Pratt algorithm (implementation taken from: https://github.com/cschen1205/cs-algorithms and slightly modified to work with bytes).

Console outputs UTF-8. Search is binary, and therefore case-sensetive. String variations (case, letter order etc) are not generated automatically because this is an opportunity to utilize multithreading easily (just generate command lines for expected variations and run several instances of the app), though it's usefullness highly depends on file size/quantity ratio.
