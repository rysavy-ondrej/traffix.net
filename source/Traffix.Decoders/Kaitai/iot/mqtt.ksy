meta:
  id: mqtt_packet
  title: MQTT is a Client Server publish/subscribe messaging transport protocol
  xref: MQTT Version 3.1.1 (http://docs.oasis-open.org/mqtt/mqtt/v3.1.1/os/mqtt-v3.1.1-os.html)
  license: MIT
  endian: be
seq:
  - id: header
    type: mqtt_fixed_header
  - id: body
    size: header.length.value
    type:
      switch-on: header.message_type
      cases:
          'mqtt_message_type::reserved_0' : mqtt_message_reserved_0
          'mqtt_message_type::connect' : mqtt_message_connect
          'mqtt_message_type::connack' : mqtt_message_connack
          'mqtt_message_type::publish' : mqtt_message_publish
          'mqtt_message_type::publish_ack' : mqtt_message_publish_ack
          'mqtt_message_type::publish_rec' : mqtt_message_publish_rec
          'mqtt_message_type::publish_rel' : mqtt_message_publish_rel
          'mqtt_message_type::publish_comp' : mqtt_message_publish_comp
          'mqtt_message_type::subscribe' : mqtt_message_subscribe
          'mqtt_message_type::subscribe_ack' : mqtt_message_subscribe_ack
          'mqtt_message_type::unsubscribe' : mqtt_message_unsubscribe
          'mqtt_message_type::unsubscribe_ack' : mqtt_message_unsubscribe_ack
          'mqtt_message_type::ping_request' : mqtt_message_ping_request
          'mqtt_message_type::ping_response' : mqtt_message_response
          'mqtt_message_type::disconnect' : mqtt_message_disconnect
          'mqtt_message_type::reserved_15' : mqtt_message_reserved_15
types:
# COMMON STRUCTURES
  mqtt_fixed_header:
    seq:
      - id: message_type
        type: b4
        enum: mqtt_message_type
      - id: dup
        type: b1
      - id: qos
        type: b2
        enum: mqtt_qos
      - id: retain
        type: b1 
      - id: length
        type: mqtt_length
  
  mqtt_length:
    seq:
      - id: bytes
        type: u1
        repeat: until
        repeat-until: '(_ & 128) == 0'
    instances:
      value:
        value: '(bytes[0] & 127) 
                + (bytes.size > 1 ? (bytes[1] & 127) * 128 : 0)
                + (bytes.size > 2 ? (bytes[2] & 127) * 128 * 128 : 0)
                + (bytes.size > 3 ? (bytes[3] & 127) * 128 * 128 * 128  : 0)'

  mqtt_connect_flags:
    seq:
      - id: username
        type: b1
      - id: password
        type: b1
      - id: will_retain
        type: b1
      - id: will_qos
        type: b2
        enum: mqtt_qos
      - id: will
        type: b1
      - id: clean_session
        type: b1
      - id: reserved
        type: b1
  mqtt_subscribe_qos:
    seq:
      - id: reserved
        type: b6
      - id: qos
        type: b2
        enum: mqtt_qos
  mqtt_subscribe_topic:
    seq:
      - id: topic_name
        type: mqtt_string
      - id: reserved
        type: b6
      - id: requested_qos
        type: b2
        enum: mqtt_qos
  mqtt_string:
    seq:
      - id: length
        type: u2
      - id: value
        type: str
        encoding: ascii
        size: length
# MESSAGE TYPES:
  mqtt_message_reserved_0:
    seq:
      - id: content_of_message
        size-eos: true
  mqtt_message_connect:
    seq:
      # Connect Header
      - id: protocol_name
        type: mqtt_string
      - id: protocol_version_number
        type: u1
      - id: connect_flags
        type: mqtt_connect_flags
      - id: keep_alive_timer
        type: u2
      # Payload
      - id:  client_id
        type: mqtt_string
      # Optional entries
      - id: will_topic
        type: mqtt_string
        if: connect_flags.will
      - id: will_message
        type: mqtt_string
        if: connect_flags.will
      - id: username
        type: mqtt_string
        if: connect_flags.username
      - id: password
        type: mqtt_string
        if: connect_flags.password
  mqtt_message_connack:
      seq:
      - id: topic_name_compression_response
        type: u1
      - id: connect_return_code
        type: u1
        enum: mqtt_connect_return_code 

  mqtt_message_publish:
      seq:
      - id: topic
        type: mqtt_string
      - id: message_id
        type: u2
        if: '_parent.header.qos == mqtt_qos::at_least_once 
            or _parent.header.qos == mqtt_qos::exactly_once'
      - id: payload
        size-eos: true
  mqtt_message_publish_ack:
      seq:
      - id: message_id
        type: u2
  mqtt_message_publish_rec:
      seq:
      - id: message_id
        type: u2
  mqtt_message_publish_rel:
      seq:
      - id: message_id
        type: u2
  mqtt_message_publish_comp:
      seq:
      - id: message_id
        type: u2
  mqtt_message_subscribe:
      seq:
      - id: message_id
        type: u2
      - id: topics
        type: mqtt_subscribe_topic
        
  mqtt_message_subscribe_ack:
      seq:
      - id: message_id
        type: u2
      - id: garanted_qos
        type: mqtt_subscribe_qos
  mqtt_message_unsubscribe:
      seq:
      - id: message_id
        type: u2
  mqtt_message_unsubscribe_ack:
      seq:
      - id: message_id
        type: u2
  mqtt_message_ping_request:
      seq:
      - id: payload   # usually empty
        size-eos: true
  mqtt_message_response:
      seq:
      - id: payload   # usually empty
        size-eos: true
  mqtt_message_disconnect:
      seq:
      - id: payload   # usually empty
        size-eos: true
  mqtt_message_reserved_15:
      seq:
      - id: content_of_message
        size-eos: true
enums:
  mqtt_message_type:
    0: reserved_0
    1: connect
    2: connack
    3: publish
    4: publish_ack
    5: publish_rec
    6: publish_rel
    7: publish_comp
    8: subscribe
    9: subscribe_ack
    10: unsubscribe
    11: unsubscribe_ack
    12: ping_request
    13: ping_response
    14: disconnect
    15: reserved_15
    
  mqtt_connect_return_code:
    0: connection_accepted                              
    1: connection_refused_unacceptable_protocol_version 
    2: connection_refused_identifier_rejected          
    3: connection_refused_server_unavailable            
    4: connection_refused_bad_username_or_password      
    5: connection_refused_not_authorized 
  
  mqtt_qos:
    0: at_most_once   
    1: at_least_once  
    2: exactly_once   
    3: reserved          
  
