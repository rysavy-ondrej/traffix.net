meta:
  id: dlms_set_request
  endian: be
  imports:
    - dlms_struct
    - dlms_data
seq:
  - id: request_type
    type: u1
    enum: set_request_type
  - id: request
    type:
      switch-on: request_type
      cases: 
        'set_request_type::set_request_normal': set_request_normal
        'set_request_type::set_request_with_first_datablock': set_request_with_first_datablock
        'set_request_type::set_request_with_datablock': set_request_with_datablock
        'set_request_type::set_request_with_list': set_request_with_list
        'set_request_type::set_request_with_list_and_first_datablock': set_request_with_list_and_first_datablock
types:
  set_request_normal:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: cosem_attribute_descriptor 
        type: dlms_struct::cosem_attribute_descriptor
      - id: access_selection
        type: dlms_struct::selective_access_descriptor_optional
      - id: value
        type: dlms_data
  set_request_with_first_datablock: 
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: datablock
        type: dlms_struct::datablock_sa
  set_request_with_datablock:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: cosem_attribute_descriptor 
        type: dlms_struct::cosem_attribute_descriptor
      - id: access_selection
        type: dlms_struct::selective_access_descriptor_optional
      - id: datablock
        type: dlms_struct::datablock_sa
  set_request_with_list: 
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: attribute_descriptor_list
        type: dlms_struct::sequence_of_cosem_attribute_descriptor_with_selection
      - id: value_list
        type: dlms_struct::sequence_of_data
  set_request_with_list_and_first_datablock:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: attribute_descriptor_list
        type: dlms_struct::sequence_of_cosem_attribute_descriptor_with_selection
      - id: datablock
        type: dlms_struct::datablock_sa
enums:
  set_request_type:
    1: set_request_normal
    2: set_request_with_first_datablock 
    3: set_request_with_datablock 
    4: set_request_with_list 
    5: set_request_with_list_and_first_datablock