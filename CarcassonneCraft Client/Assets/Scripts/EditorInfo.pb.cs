//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EditorInfo.proto
namespace CarcassonneCraft
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EditorInfo")]
  public partial class EditorInfo : global::ProtoBuf.IExtensible
  {
    public EditorInfo() {}
    
    private int _areaid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"areaid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int areaid
    {
      get { return _areaid; }
      set { _areaid = value; }
    }
    private string _username;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"username", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string username
    {
      get { return _username; }
      set { _username = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}