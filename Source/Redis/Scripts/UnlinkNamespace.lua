local result = redis.call("SCAN", 0, "MATCH", ARGV[1])
local cursor = result[1]
local keys = result[2]

while (cursor ~= "0")
do
    redis.call("UNLINK", unpack(keys))
    result = redis.call("SCAN", cursor, "MATCH", ARGV[1])
    cursor = result[1]
    keys = result[2]
end

if not not next(keys) then
    redis.call("UNLINK", unpack(keys))
end