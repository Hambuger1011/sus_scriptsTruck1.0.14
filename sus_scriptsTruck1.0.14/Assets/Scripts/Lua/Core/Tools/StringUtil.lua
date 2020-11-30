--[[
-- added by wsh @ 2017-12-18
-- string扩展工具类，对string不支持的功能执行扩展
--]]

local unpack = unpack or table.unpack

string.Empty = ''

-- 字符串分割
-- @src：被分割的字符串
-- @pattern：分隔符，可以为模式匹配
-- @init：起始位置
-- @plain：为true禁用pattern模式匹配；为false则开启模式匹配
function string.split(src, pattern, search_pos_begin, plain)
	assert(type(src) == "string")
	assert(type(pattern) == "string" and #pattern > 0)
	search_pos_begin = search_pos_begin or 1
	plain = plain or true
	local split_result = {}

	while true do
		local find_pos_begin, find_pos_end = string.find(src, pattern, search_pos_begin, plain)
		if not find_pos_begin then
			break
		end
		local cur_str = ""
		if find_pos_begin > search_pos_begin then
			cur_str = string.sub(src, search_pos_begin, find_pos_begin - 1)
		end
		split_result[#split_result + 1] = cur_str
		search_pos_begin = find_pos_end + 1
	end

	if search_pos_begin < string.len(src) then
		split_result[#split_result + 1] = string.sub(src, search_pos_begin)
	else
		split_result[#split_result + 1] = ""
	end

	return split_result
end

-- 字符串连接
function string.join(join_table, joiner)
	if #join_table == 0 then
		return ""
	end

	local fmt = "%s"
	for i = 2, #join_table do
		fmt = fmt .. joiner .. "%s"
	end

	return string.format(fmt, unpack(join_table))
end

-- 是否包含
-- 注意：plain为true时，关闭模式匹配机制，此时函数仅做直接的 “查找子串”的操作
function string.contains(target_string, pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, pattern, 1, plain)
	return find_pos_begin ~= nil
end

-- 以某个字符串开始
function string.startswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, 1, plain)
	return find_pos_begin == 1
end

-- 以某个字符串结尾
function string.endswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, -#start_pattern, plain)
	return find_pos_end == #target_string
end

--获取单个字符长度(含中文)
function string.GetCharSize(char)
	if not char then
		return 0
	elseif char > 240 then
		return 4
	elseif char > 225 then--汉字
		return 3
	elseif char > 192 then
		return 2
	else
		return 1
	end
end

 --获取中文字符长度(含中文)
function string.GetUtf8Len(str)
	local len = 0 
	local currentIndex = 1 
	while currentIndex <= #str do 
		local char = string.byte(str,currentIndex) 
		currentIndex = currentIndex + string.GetCharSize(char) 
		len = len + 1 
	end 
	return len 
end
 
--截取中文字符串(含中文)
function string.StrUtf8Sub(str, startChar, numChars)
	local startIndex = 1 
	while startChar > 1 do 
		local char = string.byte(str,startIndex) 
		startIndex = startIndex + string.GetCharSize(char) 
		startChar = startChar - 1 
	end
	
	local currentIndex = startIndex
	
	while numChars > 0 and currentIndex <= #str do
		local char = string.byte(str,currentIndex)
		currentIndex = currentIndex + string.GetCharSize(char)
		numChars = numChars - 1
	end
	
	return string.sub(str, startIndex, currentIndex-1)
end
