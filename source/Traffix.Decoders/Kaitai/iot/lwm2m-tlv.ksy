meta:
  id: lwm2m_tlv
  title: LwM2M TLV Data Format
  endian: be
  file-extension: application/vnd.oma.lwm2m+tlv
doc: >
    The binary TLV (Type-Length-Value) format is used to represent an array of values 
    or a singular value using a compact binary representation, which is easy to process 
    on simple embedded devices. The format has a minimum overhead per value of just 2 bytes 
    and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value. 
    The maximum size of an Object Instance or Resource in this format is 16.7 MB. 
    The format is self- describing, thus a parser can skip TLVs for which the Resource is not known.
    This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
  
seq:
  - id: type
    doc: "8-bits masked field"
    type: tlv_type
  - id: identifier
    doc: "The Object Instance, Resource, or Resource Instance ID as indicated by the Type field."
    type: tlv_identifier 
  - id: length
    doc: "The Length of the following field in bytes."
    type: tlv_length 
  - id: value
    doc: "Value of the tag. The format of the value depends on the Resource‟s data type."
    size: length.value

types:
  tlv_identifier:
    seq:
      - id: tlv_id_1
        type: u1
        if: _parent.type.identifier_wide_length == false
      - id: tlv_id_2
        type: u2
        if: _parent.type.identifier_wide_length == true
    instances:
      value:
        value: > 
          tlv_id_1 | tlv_id_2  
          
  tlv_length:
    seq: 
      - id: tlv_len_1
        type: u1
        if: _parent.type.length_type == 1
          
      - id: tlv_len_2
        type: b16
        if:  _parent.type.length_type == 2
  
      - id: tlv_len_3
        type: b24
        if:  _parent.type.length_type == 3
    instances:
      value:
        value: > 
          _parent.type.value_length | tlv_len_1 | tlv_len_2 | tlv_len_3 
  
  tlv_type:
    seq:
      - id: identifier_type
        doc: >
          Bits 7-6: Indicates the type of Identifier:
            00= Object Instance in which case the Value contains one or more Resource TLVs
            01= Resource Instance with Value for use within a multiple Resource TLV
            10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
            11= Resource with Value
        type: b2
        enum: lwm2m_tlv_identifier_type
      - id: identifier_wide_length
        doc: >
          Bit 5: Indicates the Length of the Identifier. 
            0=The Identifier field of this TLV is 8 bits long
            1=The Identifier field of this TLV is 16 bits long
        type: b1
      - id: length_type
        doc: >
          Bit 4-3: Indicates the type of Length.
            00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
            01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
            10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
            11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
        type: b2
      - id: value_length
        doc: >
          Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
        type: b3
        
enums:
  lwm2m_tlv_identifier_type:
    0: object_instance
    1: resource_instance
    2: multiple_resource
    3: resource_with_value
