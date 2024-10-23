local matches = redis.call("ZRANGE", KEYS[1], "-inf", "inf", "BYSCORE")
for _, keys in matches do
    redis.call("UNLINK", key(KEYS[1], key))
end
redis.call("ZREMRANGEBYSCORE", KEYS[1], "-inf", "inf")