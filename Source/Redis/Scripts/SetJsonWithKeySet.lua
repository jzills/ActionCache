redis.call("SET",  KEYS[1] .. ":" .. KEYS[2], ARGV[1])
redis.call("SADD", KEYS[1], KEYS[2])