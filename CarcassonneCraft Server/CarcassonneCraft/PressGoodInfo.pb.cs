//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: PressGoodInfo.proto
namespace CarcassonneCraft
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PressGoodInfo")]
  public partial class PressGoodInfo : global::ProtoBuf.IExtensible
  {
    public PressGoodInfo() {}
    
    private int _areaid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"areaid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int areaid
    {
      get { return _areaid; }
      set { _areaid = value; }
    }
    private bool _good;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"good", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool good
    {
      get { return _good; }
      set { _good = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}