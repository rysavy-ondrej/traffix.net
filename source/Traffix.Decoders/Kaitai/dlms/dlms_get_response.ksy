meta:
  id: dlms_get_response
  endian: be
  imports:
    - dlms_data
    - dlms_struct
seq:
  - id: response_type
    type: u1
    enum: get_response_type
  - id: response
    type:
      switch-on: response_type
      cases:
        'get_response_type::get_response_normal': get_response_normal
        'get_response_type::get_response_next': get_response_with_datablock
        'get_response_type::get_response_with_list': get_response_with_list
types:
  get_response_normal:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority    
      - id: result
        type: dlms_struct::get_data_result
  get_response_with_datablock:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority  
      - id: result
        type: dlms_struct::datablock_g
  get_response_with_list:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority  
      - id: result
        type: dlms_struct::sequence_of_get_data_result
enums:
  get_response_type:
    1: get_response_normal
    2: get_response_next
    3: get_response_with_list