local result = redis.call("SMEMBERS", KEYS[1])
for _, value in ipairs(result) do
    redis.call("UNLINK", KEYS[1] .. ":" .. value)
    redis.call("SREM", KEYS[1], value)
end