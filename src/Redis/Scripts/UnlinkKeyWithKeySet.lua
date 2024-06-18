redis.call("UNLINK", KEYS[1] .. ":" .. KEYS[2])
redis.call("SREM", KEYS[1], KEYS[2])