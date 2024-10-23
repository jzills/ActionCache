redis.call("HSET",  KEYS[1] .. ":" .. KEYS[2], "VALUE", ARGV[1], "ABSOLUTE_EXPIRATION", ARGV[2], "SLIDING_EXPIRATION", ARGV[3])
redis.call("ZADD", KEYS[1], ARGV[2], KEYS[2])
redis.call("PEXPIRE", KEYS[1] .. ":" .. KEYS[2], ARGV[3])