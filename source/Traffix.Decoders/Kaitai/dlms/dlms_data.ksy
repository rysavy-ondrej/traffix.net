meta:
  id: dlms_data
  endian: be
seq: 
  - id: datatype
    type: u1
    enum: data_type
  - id: data_value
    type:
      switch-on: datatype
      cases: 
        'data_type::null_data': null_data
        'data_type::array': array
        'data_type::structure': structure
        'data_type::boolean': boolean
        'data_type::bit_string': bit_string
        'data_type::double_long':  double_long
        'data_type::double_long_unsigned':  double_long_unsigned
        'data_type::octet_string':  octet_string
        'data_type::visible_string':  visible_string
        'data_type::bcd':  bcd
        'data_type::integer':  integer
        'data_type::long':  long
        'data_type::unsigned':  unsigned
        'data_type::long_unsigned':  long_unsigned
        'data_type::compact_array':  compact_array
        'data_type::long64':  long64
        'data_type::long64_unsigned':  long64_unsigned
        'data_type::enum':  enum
        'data_type::float32':  float32
        'data_type::float64':  float64
        'data_type::date_time':  date_time
        'data_type::date':  date
        'data_type::time':  time
        'data_type::do_not_care':  do_not_care 
types:
  null_data:
    seq:
      - id: nothing
        size: 0
  array:
    seq:
      - id: length
        type: length_encoded
      - id: value
        size: length.value
  structure:
    { }
  boolean_optional:
    seq:
      - id: present
        type: u1
      - id: value
        type: boolean
        parent: false
        if: present != 0
  boolean:
    seq:
      - id: value
        type: u1
  bit_string:
    seq:
      - id: length
        type: length_encoded
      - id: value
        type: u1
        repeat: expr
        repeat-expr: length.value
  double_long:
     seq:
      - id: value
        type: s4 
  double_long_unsigned:
     seq:
      - id: value
        type: u4 
  octet_string_optional:
    seq:
      - id: present
        type: u1
      - id: value
        type: octet_string
        parent: false
        if: present != 0
  octet_string:
    seq:
      - id: length
        type: length_encoded
      - id: value
        size: length.value
  visible_string:
    seq:
      - id: length
        type: length_encoded
      - id: value
        type: str
        encoding: ascii
        size: length.value
  bcd:
    {}
  integer:
     seq:
      - id: value
        type: s1   
  long:
     seq:
      - id: value
        type: s2   
  unsigned:
     seq:
      - id: value
        type: u1   
  long_unsigned:
     seq:
      - id: value
        type: u2   
  compact_array:
    {}
  long64:
     seq:
      - id: value
        type: s8    
  long64_unsigned:
     seq:
      - id: value
        type: u8      
  enum:
     seq:
      - id: value
        type: u1   
  float32:
    seq:
      - id: value
        type: f4
  float64:
    seq:
      - id: value
        type: f8
  date_time:
    seq:
      - id: value
        size: 12
  date:
    seq:
      - id: value
        size: 5
  time:
    seq:
      - id: value
        size: 4
  do_not_care:
    {}
  length_encoded:
    seq:
      - id: b1
        type: u1
      - id: int2
        type: u2be
        if: b1 == 0x82
    instances:
      value:
        value: '(b1 & 0x80 == 0) ? b1 : int2' 
enums:
  data_type:
    0: null_data
    1: array
    2: structure
    3: boolean
    4: bit_string
    5: double_long
    6: double_long_unsigned
    9: octet_string
    10: visible_string
    13: bcd
    15: integer
    16: long
    17: unsigned
    18: long_unsigned
    19: compact_array
    20: long64
    21: long64_unsigned
    22: enum
    23: float32
    24: float64
    25: date_time
    26: date
    27: time
    255: do_not_care
