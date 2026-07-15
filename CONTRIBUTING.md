# Contributing to FAISS.Sharp

![Welcome to FAISS.Sharp](Images/img4.png)

First off, thank you for considering contributing to **FAISS.Sharp**! It's people like you that make the open-source community such an amazing place to learn, inspire, and create. 

FAISS.Sharp is maintained by **OzzieAI**, and we welcome contributions from developers of all skill levels to improve this C# .NET 10 wrapper for FAISS.

## Where to Find Support & Discussions

Before submitting an issue or Pull Request, you might want to discuss your ideas or get help from the community:
* **Website**: [https://www.ozzieai.com/](https://www.ozzieai.com/)
* **Support Forum**: [https://forum.ozzieai.com/](https://forum.ozzieai.com/)

## How to Contribute

![Development Workflow](Images/img5.png)

### 1. Reporting Bugs
If you find a bug, please create an issue on our repository. Include as much detail as possible to help us resolve it quickly:
* Your OS and environment details.
* Confirmation that you are using .NET 10.
* Steps to reproduce the bug.
* Expected behavior vs. actual behavior.
* Code snippets or a minimal reproducible example.

### 2. Suggesting Enhancements
We are always looking to improve our FAISS wrapper. If you have an idea for a new feature (such as supporting a newly released FAISS index type) or a performance enhancement, please open an issue and describe your proposal.

### 3. Submitting Pull Requests
1. Fork the repository and create a feature branch from `main`.
2. Ensure you have the .NET 10 SDK installed.
3. Make your changes in your branch.
4. Ensure any new features include appropriate XML documentation and unit tests.
5. Build and test the project locally.
6. Issue a Pull Request with a clear title and description explaining the "why" and "what" of your changes.

## Development Setup

![Architecture & Testing](Images/img6.png)

To build FAISS.Sharp locally:
1. Clone your fork of the repository.
2. Install the **.NET 10 SDK**.
3. Ensure you have the native FAISS C-API binaries accessible in your build path (e.g., `faiss_c.dll` on Windows, or `libfaiss_c.so` on Linux).
4. Run `dotnet build` to compile the wrapper.
5. Run `dotnet test` to execute the test suite and ensure no regressions were introduced.

## Code Style & Guidelines
* We follow standard Microsoft C# coding conventions.
* Please ensure you do not leave any compiler warnings.
* Use XML documentation comments for all public APIs (e.g., `/// <summary>`) so that IntelliSense provides a great experience for end-users.
* When working with native memory and pointers, always ensure resources are properly wrapped in `SafeHandle` (like `FaissIndexHandle` or `VectorTransformHandle`) to prevent memory leaks.

Thank you for helping us make FAISS.Sharp the best vector search library for the .NET ecosystem!