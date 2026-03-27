# RSUV Bit Packer
Components and tools to facilitate the packing of RSUV (Renderer Shader User Value).

## Problem Statement
Using _**Renderer Shader User Value**_ (RSUV), introduced in **Unity 6.3**, allows setting unique properties per renderer, to be used in their material shaders, with no performance cost when using the _SRP Batcher_ (and _GPU Resident Drawer_).

RSUV being a ```uint```, it requires packing data on the C# renderer side, and unpacking data on the HLSL shader side.
While this is trivial, it requires defining packing schemes, and some data management on both ends.

## Solution
RSUV Bit Packer aims at providing a user friendly workflow to design packing scheme, set and get properties data, in the Editor and at Runtime from C# scripts and/or animation.

### RSUV Property Packer
_RSUV Property Packer_ is a component that allows listing and setting 'Renderer Properties'.
![alt text](Documentation~\Property Packer.gif)
Properties are packed and set on the renderer when modified and/or animated in the Editor and at Runtime.
The API ```RSUVPropertyPacker.TrySetProperty<>()``` allows setting properties from C# at Runtime.
The list of Renderer Properties can be defined in the component, or inherited from a _RSUV Property Sheet_.

### RSUV Property Sheet
RSUV Property Sheet is an asset that allows defining a packing scheme and properties default values, to be reused with several RSUV Property Packers.
It also allows generating a _Shader Include_ (HLSL) to access the properties in _Shader Graph_.

#### Shader Includes
In Unity 6.3 and 6.4, Shader Includes are generated using the _Shader Graph Custom Function_ syntax.
In Unity 6.5, Shader Includes are generated using the _Shader Function Reflection API_ syntax, which makes them automatically accessible in the Shader Graph without having to manually configure a _Custom Function Node_.

### Extensions
RSUV being implemented only on some ```Renderer``` classes, such as ```MeshRenderer``` and ```SkinnedMeshRenderer```, this package contains an Extension that makes it easy to set the ```ShaderUserValue``` on a ```Renderer```.

### Writing Renderer Property Types
The package is easily extensible to add new Renderer Property types by simply providing their data encoding (C#) and decoding (HLSL).

