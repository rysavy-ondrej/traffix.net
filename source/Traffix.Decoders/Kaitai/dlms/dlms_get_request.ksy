meta:
  id: dlms_get_request
  endian: be
  imports:
    - dlms_data
    - dlms_struct
seq:
  - id: request_type 
    type: u1
    enum: get_request_type
  - id: request
    type:
      switch-on: request_type
      cases: 
        'get_request_type::get_request_normal': get_request_normal
        'get_request_type::get_request_next': get_request_next
        'get_request_type::get_request_with_list': get_request_with_list
types:
  get_request_normal:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: cosem_attribute_descriptor
        type: dlms_struct::cosem_attribute_descriptor
      - id: access_selection
        type: dlms_struct::selective_access_descriptor_optional
  get_request_next:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: block_number
        type: u4
  get_request_with_list:
    seq:
      - id: invoke_id_and_priority
        type: dlms_struct::invoke_id_and_priority
      - id: attribute_descriptor_list
        type: dlms_struct::sequence_of_cosem_attribute_descriptor_with_selection
enums:
  get_request_type:
    1: get_request_normal
    2: get_request_next
    3: get_request_with_list  