local result = redis.call("SCAN", 0, "MATCH", ARGV[1])
local cursor, keys = unpack(result)

while (cursor ~= "0")
do
    redis.call("UNLINK", unpack(keys))
    result = redis.call("SCAN", cursor, "MATCH", ARGV[1])
    cursor, keys = unpack(result)
end

if next(keys) then
    redis.call("UNLINK", unpack(keys))
end