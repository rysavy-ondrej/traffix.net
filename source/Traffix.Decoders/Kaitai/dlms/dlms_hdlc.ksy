meta:
  id: dlms_hdlc
  endian: be
doc: |
    This is parser for the data link layer for the 3-layer, connection-oriented, HDLC-based,
    asynchronous communication COSEM profile. 

    DLMS information is carried in the information field. To parse the informaiton 
    field use dlms_acse or dlms_apdu. What to use depends on the first bbyte of information octet string:

    * 91-101: dlms_acse
    * 192-199: dlms_apdu

    In order to ensure a coherent data link layer service specification for both connection-oriented and
    connectionless operation modes, the data link layer is divided into two sub-layers: the Logical Link
    Control (LLC) sub-layer and the Medium Access Control (MAC) sub-layer.
    
    The LLC sub-layer is based on ISO/IEC 8802-2. 
    
    The MAC sub-layer – the major part of this data link layer specification – is based on ISO/IEC13239 (HDLC). 
    
    HDLC Addressing:
    
    The HDLC frame format type 3 contains two address fields: a destination and a
    source HDLC address. Depending on the direction of the data transfer, both the client and the
    server addresses can be destination or source addresses.
    The client address shall always be expressed on one byte.
    The server address – to enable addressing more than one logical device within a single physical
    device and to support the multi-drop configuration – may be divided into two parts:
    - the upper HDLC address is used to address a Logical Device (a separately addressable entity within a physical device);
    - the lower HDLC address is used to address a Physical Device (a physical device on the multi-drop).
      The upper HDLC address shall always be present. The lower HDLC address may be omitted if it is
      not required. 
    The HDLC address extension mechanism applies to both parts. This mechanism specifies variable
    length address fields, but for the purpose of this protocol, the length of a complete server address 
    field is restricted to be one, two or four bytes long.
    
seq:
  - id: start_flag 
    contents: [ 0x7e ]
    doc: |
      The flag field is one byte and its value is 7E.
  - id: hdlc_header
    type: hdlc_header_fields
    doc: |
      The MAC sub-layer uses the HDLC frame format type 3 as defined in Annex H.4 of ISO/IEC 13239. 
  - id: llc_header
    type: llc_header_fields
    if: 'hdlc_header.control.frame_type & 0x1 == 0' 
    doc: |
      The LLC sub-layer transmits LSDUs transparently between its service user layer and the MAC sublayer.
      LLC is used only with I-frames.

  - id: information
    size: |
      hdlc_header.format.frame_length - (hdlc_header.size + (hdlc_header.control.frame_type & 0x1 == 0 ? llc_header.size : 0) + 2)
    doc: |
      The information field may be any sequence of bytes. In the case of data frames (I and UI frames), it carries the MSDU. 
  - id: fsc
    type: u2
    doc: |
      Unless otherwise noted, the frame checking sequence is
      calculated for the entire length of the frame, excluding the opening flag, the FCS and any start and
      stop elements (start/stop transmission).
  - id: stop_flag
    contents: [ 0x7e ]
    doc: |
      The flag field is one byte and its value is 7E. 
      When two or more frames are
      transmitted continuously, a single flag is used as both the closing flag of one frame and the
      opening flag of the next frame.
      
types:
  hdlc_header_fields:
    seq:
      - id: format
        type: format_type
        size: 2
        doc: |
          The length of the frame format field is two bytes. It consists of three sub-fields referred to as the
          Format type sub-field (4 bits), the Segmentation bit (S, 1 bit) and the frame length sub-field (11 bits).
      - id: dst_address
        type: hdlc_address
        doc: | 
            Depending on the direction of the data transfer, both the client and the
            server addresses can be destination or source addresses.
            The HDLC address extension mechanism applies to address representation.
            The client address is only 1 byte. Server address can be 1,2, 4 bytes.          
      - id: src_address
        type: hdlc_address
        doc: | 
          Depending on the direction of the data transfer, both the client and the
          server addresses can be destination or source addresses. 
          The HDLC address extension mechanism applies to address representation.
          The client address is only 1 byte. Server address can be 1,2, 4 bytes.
      - id: control
        type: control_type
        size: 1
        doc: |
           It indicates the type of commands or responses, and
            contains sequence numbers, where appropriate.
      - id: hcs
        type: u2
        if: 'control.frame_type & 0x1 == 0' 
        doc: |
          This check sequence is applied to only the header, i.e.,
          the bits between the opening flag sequence and the header check sequence.
          Frames that do not have an information field or have an empty information field, e.g., as with some supervisory frames,
          do not contain an HCS. 
    instances:
      size:
        value: '2 + dst_address.size + src_address.size + 1 + (control.frame_type & 0x1 == 0 ? 2 : 0)'
    
    
  llc_header_fields:
    seq:
      - id: remote_lsap
        contents: [ 0xe6 ]
        if: 'llc_start_byte == 0xe6'
        doc: |
          Destination_LSAP is always 0xE6.
      - id: local_lsap
        type: u1
        enum: llc_packet_type
        if: 'llc_start_byte == 0xe6'
        doc: |
          The value of the Source_LSAP is 0xE6 or 0xE7. The last bit is used as a command/response identifier:
          0xE6 ‘command’ and 0xE7 “response”. 
      - id: llc_quality
        contents: [ 0 ]
        if: 'llc_start_byte == 0xe6'
        doc: |
          The quality value is reserved for future use and must be 0.
    instances:
      size:
        value: 'llc_start_byte == 0xe6 ? 3 : 0'
      llc_start_byte:
        pos: _io.pos
        type: u1
      
  format_type:
    seq:
      - id: frame_format_type
        type: b4
        doc: |
          The value of the format type sub-field is 1010 (binary), which identifies a frame format type 3.
      - id: segmentation_flag
        type: b1
        doc: |
          Rules of using the segmentation bit are defined in the complete Green Book (?).
      - id: length
        type: b11
        doc: | 
          The value of the frame length subfield is the count of octets in the frame excluding the opening and
          closing frame flag sequences. 
    instances:
      frame_length:
        value: length + 0 # (OR) this should convert length from ulong to int; otherwise the C# parser will not compile.
  control_type:
    seq:
      - id: i_frame
        type: i_frame_control_byte
        if: 'frame_type & 0x1 == 0' 
      - id: s_frame
        type: s_frame_control_byte
        if: 'frame_type == 1'
      - id: u_frame
        type: u_frame_control_byte
        if: 'frame_type == 3'
    instances:
      control_byte:
        pos: 0
        type: u1
      frame_type:
        value: 'control_byte & 0x3'
  i_frame_control_byte:
    seq:
      - id: recv_sequence_number
        type: b3
      - id: pf_bit
        type: b1
        enum: hdlc_pf_bit
      - id: send_sequence_number
        type: b3
  s_frame_control_byte:
    seq:
      - id: recv_sequence_number
        type: b3
      - id: pf_bit
        type: b1
        enum: hdlc_pf_bit
      - id: s_frame_type
        type: b2        
        enum: s_frame_type
  u_frame_control_byte:
    seq:
      - id: r_type
        type: b3
      - id: pf_bit
        type: b1
        enum: hdlc_pf_bit
      - id: s_type
        type: b2
  hdlc_address:   
    doc: |
      This mechanism specifies variable
      length address fields, but for the purpose of this protocol, the length of a complete server address
      field is restricted to be one, two or four bytes long. 
      
      The address field range can be extended by
      reserving the first transmitted bit (low-order) of each address octet which would then be set to
      binary zero to indicate that the following octet is an extension of the address field. 
      
      The format of the extended octet(s) shall be the same as that of the first octet. Thus, the address field may be
      recursively extended. The last octet of an address field is indicted by setting the low-order bit to
      binary one.
      
      When extension is used, the presence of a binary "1" in the first transmitted bit of the first address
      octet indicates that only one address octet is being used. The use of address extension thus
      restricts the range of single octet addresses to 0x7F and for two octet addresses to 0…0x3FFF.
      
      Single bytes address:
      | address-7bits | 1 | 
      
      Two bytes address: 
      | address-7bits | 0 |  | address-7bits | 1 | 
      
      Four bytes address:
      | address-7bits | 0 |   | address-7bits | 0 |   | address-7bits | 0 |  | address-7bits | 1 | 
      
    seq:
      - id: bytes
        type: hdlc_address_byte
        repeat: until
        repeat-until: not _.has_next
    instances:
          size:
            value: bytes.size
          last:
            value: bytes.size - 1
          value:
            value: >-
              bytes[last].value
              + (last >= 1 ? (bytes[last - 1].value << 7) : 0)
              + (last >= 2 ? (bytes[last - 2].value << 14) : 0)
              + (last >= 3 ? (bytes[last - 3].value << 21) : 0)
            doc: Resulting value as normal integer
  hdlc_address_byte:
    doc: |
      One byte group, clearly divided into 7-bit "value" and 1-bit "has continuation
      in the next byte" flag. contiune with next byte = 0, final byte = 1.
    seq:
      - id: b
        type: u1
    instances:
      has_next:
        value: (b & 0b0000_0001) == 0
        doc: If true, then we have more bytes to read
      value:
        value: (b & 0b111_11110) >> 1
        doc: The 7-bit (base128) numeric value of this group
    
enums:
  frame_type_enum:  
    1: s_frame
    2: i_frame
    3: u_frame
  s_frame_type:
    0: receiver_ready
    1: receiver_not_ready
    2: reject
    3: selective_reject
  hdlc_cr:
    0: hdlc_command
    1: hdcl_response
  llc_packet_type:
    0xe6: llc_command
    0xe7: llc_response
  hdlc_pf_bit:
    0: hdlc_poll
    1: hdlc_final
