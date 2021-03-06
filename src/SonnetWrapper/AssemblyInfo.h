// Copyright (C) Jan-Willem Goossens 
// This code is licensed under the terms of the Eclipse Public License (EPL).

#pragma once

#define VER_FILEVERSION			1,4,0,0
#define VER_FILEVERSION_STR		"1.4.0.0"

// About Production & Assembly version:
// Did the interface change? 
// -> Yes: Are the changes backward compatible?
//         -> No: Then change the version number
//         -> Yes: Keep same version. 
// -> No: Keep same version.
#define VER_PRODUCTVERSION		1,4,0,0
#define VER_PRODUCTVERSION_STR	"1.4.0.0"
#define VER_ASSEMBLYVERSION_STR	"1.4.0.0" // Can use *
// Copyright (C) Jan-Willem Goossens 
// This code is licensed under the terms of the Eclipse Public License (EPL).

#ifdef _DEBUG
#ifndef WIN32
#define VER_FILEDESCRIPTION "SonnetWrapper 64-bit (Debug)"
#else
#define VER_FILEDESCRIPTION "SonnetWrapper 32-bit (Debug)"
#endif
#else
#ifndef WIN32
#define VER_FILEDESCRIPTION "SonnetWrapper 64-bit"
#else
#define VER_FILEDESCRIPTION "SonnetWrapper 32-bit"
#endif
#endif

#define VER_COPYRIGHT "Copyright (C) 2011-2021"
#define VER_TRADEMARK "This code is licensed under the terms of the Eclipse Public License (EPL)"
#define VER_FILENAME "SonnetWrapper.dll"
#define VER_FILECOMMENTS "SonnetWrapper is a managed DLL with wrapper classes around existing C++ COIN-OR classes. This version of SonnetWrapper is based on Cbc 2.9.3. See http://sourceforge.net/projects/sonnet-project and http://www.coin-or.org."
