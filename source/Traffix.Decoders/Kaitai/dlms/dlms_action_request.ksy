meta:
  id: dlms_action_request
  endian: be
  imports:
    - dlms_data
    - dlms_struct
seq:
    - id: request_type 
      type: u1
      enum: action_request_type
    - id: request
      type:
        switch-on: request_type
        cases: 
            'action_request_type::action_request_normal': action_request_normal
            'action_request_type::action_request_next_pblock': action_request_next_pblock
            'action_request_type::action_request_with_list': action_request_with_list
            'action_request_type::action_request_with_first_pblock': action_request_with_first_pblock
            'action_request_type::action_request_with_list_and_first_pblock': action_request_with_list_and_first_pblock
            'action_request_type::action_request_with_pblock': action_request_with_pblock
types:
    action_request_normal:
      seq:
        - id: invoke_id_and_priority
          type: dlms_struct::invoke_id_and_priority
        - id: cosem_method_descriptor
          type: dlms_struct::cosem_method_descriptor
        - id: method_invocation_parameters
          type: dlms_struct::data_optional
    action_request_next_pblock:
      seq:
        - id: invoke_id_and_priority
          type: dlms_struct::invoke_id_and_priority
        - id: block_number
          type: u4
    action_request_with_list:
        {}
    action_request_with_first_pblock:
        {}
    action_request_with_list_and_first_pblock:
        {}
    action_request_with_pblock:
        {}

enums:
    action_request_type:
        1: action_request_normal
        2: action_request_next_pblock
        3: action_request_with_list
        4: action_request_with_first_pblock 
        5: action_request_with_list_and_first_pblock 
        6: action_request_with_pblock