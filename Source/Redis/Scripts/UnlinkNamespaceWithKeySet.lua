local key = function(namespace, value) 
    return namespace .. ":" .. value
end

local unlinkRemove = function(namespace, value)
    redis.call("UNLINK", key(namespace, value))
    redis.call("SREM", namespace, value)
end

local unlinkRemoveAll = function(table)
    for _, value in ipairs(table) do
        unlinkRemove(KEYS[1], value)
    end
end

local size = redis.call("SCARD", KEYS[1])
if (size > 100) then
    local result = redis.call("SSCAN", KEYS[1], 0)
    local cursor, values = unpack(result)
    if (cursor == "0") then
        unlinkRemoveAll(values)
    else
        while (cursor ~= "0")
        do
            unlinkRemoveAll(values)
            result = redis.call("SSCAN", KEYS[1], cursor)
            cursor, values = unpack(result)
        end
    end
else
    local result = redis.call("SMEMBERS", KEYS[1])
    unlinkRemoveAll(result)
end