meta:
  id: dlms_apdu
  endian: be
  imports: 
    - dlms_get_request
    - dlms_get_response
    - dlms_set_request
    - dlms_set_response
    - dlms_event_notification_request
    - dlms_action_request
    - dlms_action_response
seq:
  - id: pdu_type
    type: u1
    enum: dlms_pdu_type
  - id: pdu 
    size-eos: true
    type:
      switch-on: pdu_type
      cases:
        'dlms_pdu_type::get_request': dlms_get_request
        'dlms_pdu_type::get_response': dlms_get_response
        'dlms_pdu_type::set_request': dlms_set_request
        'dlms_pdu_type::set_response': dlms_set_response   
        'dlms_pdu_type::even_notification_request': dlms_event_notification_request
        'dlms_pdu_type::action_request': dlms_action_request
        'dlms_pdu_type::action_response': dlms_action_response
enums:
  dlms_pdu_type:
    192: get_request 
    193: set_request
    194: even_notification_request
    195: action_request
    196: get_response
    197: set_response
    199: action_response