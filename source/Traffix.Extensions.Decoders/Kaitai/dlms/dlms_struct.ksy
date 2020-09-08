meta:
  id: dlms_struct
  endian: be
  imports:
    - dlms_data
types:
  invoke_id_and_priority:
    seq:
      - id: priority
        type: b1
      - id: service_class
        type: b1
      - id: invoke_id
        type: b6
  cosem_attribute_descriptor_with_selection:
    seq:
      - id: cosem_attribute_descriptor 
        type: cosem_attribute_descriptor
      - id: access_selection
        type: selective_access_descriptor_optional
  cosem_attribute_descriptor:
    seq:
      - id: class_id
        type: u2
      - id: instance_id
        type: cosem_object_instance_id
      - id: attribute_id
        type: u1
  cosem_method_descriptor:
    seq:
      - id: class_id
        type: u2
      - id: instance_id
        type: cosem_object_instance_id
      - id: method_id
        type: u1
  cosem_object_instance_id:
    seq:
      - id: oid
        size: 6
  cosem_date_time_optional:
    seq:
      - id: present
        type: u1
      - id: value
        type: cosem_date_time
        if: present != 0
  cosem_date_time:
    seq:
      - id: bytes
        size: 12
  data_access_result:
    seq:
      - id: value
        type: u1
        enum: access_result
  datablock_g:
    seq:
      - id: last_block
        type: u1
      - id: block_number
        type: u4
      - id: result_choice
        type: u1
      - id: result
        type:
          switch-on: result_choice
          cases:
            0: dlms_data::octet_string
            1: data_access_result
  datablock_sa:
    seq:
      - id: data
        size-eos: true
  get_data_result_optional:
    seq: 
      - id: present
        type: u1
      - id: value
        type: get_data_result
        if: present != 0          
  get_data_result:
    seq: 
      - id: data_result_type
        type: u1
      - id: data_result_value 
        type:
          switch-on: data_result_type
          cases:
            0: dlms_data
            1: data_access_result
  selective_access_descriptor_optional:
    seq:
      - id: present
        type: u1
      - id: value
        type: selective_access_descriptor
        if: present != 0
  selective_access_descriptor:
    seq:
      - id: access_selector
        type: u1
      - id: access_parameters
        type: dlms_data
  sequence_of_cosem_attribute_descriptor_with_selection:
    seq:
      - id: item_count
        type: length_encoded
      - id: items
        type: cosem_attribute_descriptor_with_selection
        repeat: expr
        repeat-expr: item_count.value
  data_optional:
    seq: 
      - id: present
        type: u1
      - id: value
        type: dlms_data
        if: present != 0
  sequence_of_data:
    seq:
      - id: item_count
        type: length_encoded
      - id: items
        type: dlms_data
        repeat: expr
        repeat-expr: item_count.value
  sequence_of_get_data_result:
    seq:
      - id: item_count
        type: length_encoded
      - id: items
        type: get_data_result
        repeat: expr
        repeat-expr: item_count.value
  sequence_of_data_access_result:
    seq:
      - id: item_count
        type: length_encoded
      - id: items
        type: data_access_result
        repeat: expr
        repeat-expr: item_count.value    
  length_encoded:
    seq:
      - id: b1
        type: u1
      - id: int2
        type: u2be
        if: b1 == 0x82
      - id: int4
        type: u4be
        if: b1 == 0x84
    instances:
      value:
        value: '(b1 == 0x84) ? int4 : (b1 == 0x82) ? int2 : b1'
enums:
  access_result:
    0: success
    1: hardware_fault
    2: temporary_failure
    3: read_write_denied
    4: object_undefined
    9: object_class_inconsistent
    11: object_unavailable 
    12: type_unmatched 
    13: scope_of_access_violated
    14: data_block_unavailable 
    15: long_get_aborted 
    16: no_long_get_in_progress
    17: long_set_aborted
    18: no_long_set_in_progress
    250: other_reason
