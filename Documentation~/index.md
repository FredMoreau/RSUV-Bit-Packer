# RSUV Bit Packer
Using _**Renderer Shader User Value**_ (RSUV), introduced in **Unity 6.3**, allows setting unique properties per renderer, to be used in their material shaders, with no performance cost when using the _SRP Batcher_ (and _GPU Resident Drawer_).

The RSUV is a ```uint```, which requires packing data on the C# renderer side, and unpacking data on the HLSL shader side.
While packing data in a 32 bit unsigned integer with C# and unpacking the data with HLSL is trivial, the RSUV Bit Packer (package) aims at providing a user friendly workflow to design packing scheme.

The package allows you to define a packing scheme by adding renderer properties, easily changing precision to fit in the 32 bits.
The Property Sheet asset can generate a _**Shader Include**_ using the new _**Shader Function Reflection API**_ to fetch the data in _**Shader Graph**_.
The Property Packer component sets the value on renderers and exposes properties to Animation, C# and even Visual Scripting.

### Property Packer
_RSUV Property Packer_ is a component that allows listing and setting 'Renderer Properties'.

![Adding properties to a Property Packer.](./PropertyPacker.png)

Properties are packed and set on the renderer when modified and/or animated in the Editor and at Runtime.
The API ```RSUVPropertyPacker.TrySetProperty<>()``` allows setting properties from C# at Runtime.
The list of Renderer Properties can be defined in the component, or inherited from a _RSUV Property Sheet_.

### Property Sheet
RSUV Property Sheet is an asset that allows defining a packing scheme and properties default values, to be reused with several RSUV Property Packers.

![Adding properties to a Property Sheet.](./PropertySheet.png)

It also allows generating a _Shader Include_ (HLSL) to access the properties in _Shader Graph_.

Assigning a Property Sheet on a Property Packer will make it inherits the properties from the sheet.

![Setting a Property Packer with a Property Sheet.](./PropertyPackerWithPropertySheet.png)

#### Shader Includes
##### Unity 6.3 - 6.4
In Unity 6.3 and 6.4, Shader Includes are generated using the _Shader Graph Custom Function_ syntax.
##### Unity 6.5 and above
In Unity 6.5, Shader Includes are generated using the _Shader Function Reflection API_ syntax, which makes them automatically accessible in Shader Graph without having to manually configure a _Custom Function Node_.

### Capacity
Some UX to help you stay within the 32 bits limit.

A counter at the top tells you how many bits are used, and the Add Menu grays out the fixed size properties that cannot fit.

![Cannot add more properties.](./PropertySheet_Limited_Add.png)

![Max Capacity.](./PropertySheet_MaxCapacity.png)

The size of most properties can be adjusted to fit.
If the properties exceed the capacity, the Helpbox turns into a warning, and any property that doesn't fit turns red.

![Over Capacity.](./PropertySheet_OverCapacity.png)

### Color Palette
A Color Palette is an asset that allows defining colors and generating an HLSL Shader Include.

![Defining a Color Palette.](./ColorPalette.png)

The generated HLSL allows getting a color from the palette in Shader Graph.

![Getting color from Palette in Shader Graph.](./ColorPaletteNode.png)

### Samples
#### Examples
#### More Renderer Properties
#### Visual Scripting

### Extensions
RSUV being implemented only on some ```Renderer``` classes, such as ```MeshRenderer``` and ```SkinnedMeshRenderer```, this package contains an Extension that makes it easy to set the ```ShaderUserValue``` on a ```Renderer```.

### Writing Renderer Property Types
The package is easily extensible to add new Renderer Property types by simply providing their data encoding (C#) and decoding (HLSL).

```
using UnityEngine.RSUVBitPacker;
// RendererValue Attributes are used by the PropertyList.
[System.Serializable]
[RendererValueTypeName("NewRendererProperty")] //The name of the 'Add' Dropdown MenuItem.
//[RendererValueTypeLength(1)] // If used, the MenuItem will be grayed out if the length is greater than remaining bits available.
[RendererValueTypeTooltip("A single bit storing a boolean value.")] // 
public class NewRendererProperty : RendererProperty<bool> // the type defines the serialized value type
{
    // This is used to offset bitshift index.
    public override uint Length => 1;
    
    // This is expected to provide the data, as a uint, of which only the first n bits are used.
    public override uint Data => Value ? 1u : 0u;

    // The next two overrides are not mandatory. They are used by the HLSL generator.
    // If left unoverriden, the property value will not be featured in the generated Shader Include.
    // This is used to write the 'out' parameter.
    public override string HlslType => "bool";

    // This is used to write the parameter assignment in the shader function body.
    // rsuv is short for unity_RendererUserValue.
    // paramName is the name of the property as set by the user.
    // bitIndex is where the data storage begins in the rsuv uint.
    public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (rsuv & (1 << {bitIndex})) != 0;";
}
```

### Known limitations and potential future improvements
#### Property Names
There is no check on property names other than whitespace removal.
Don't give several properties the same name, and don't name properties with HLSL types or intrisic functions like "float" or "dot".

#### IPropertyProvider
Considered for future improvement to add an interface for MonoBehaviours to provide renderer properties.

#### HLSL Namespacing
The generated includes are not namespaced. Adding namespacing may avoid collisions with other functions.

