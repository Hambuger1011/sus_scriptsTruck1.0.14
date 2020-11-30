System.Array = {}
local Array = core.Class("System.Array")
System.Array = Array
function Array:__init()
    System.ICloneable.implements(self)
    System.Collections.ICollection.implements(self)
end

function Array:Count()
    return 0
end

function Array:CopyTo(array,startIndex)
end

local DoBinarySearch = function(array, index, length, value, comparer)
    local iMin = index;
    --Comment from Tum (tum@veridicus.com):
    --*Must* start at index + length - 1 to pass rotor test co2460binarysearch_iioi
    local iMax = index + length - 1
    local iCmp = 0

    while (iMin <= iMax) do
        -- Be careful with overflow
        -- http://googleresearch.blogspot.com/2006/06/extra-extra-read-all-about-it-nearly.html
        local iMid = iMin + ((iMax - iMin) / 2)
        local elt = array.Get(iMid)

        iCmp = comparer(elt, value)

        if (iCmp == 0) then
            return iMid
        elseif (iCmp > 0) then
            iMax = iMid - 1
        else
            iMin = iMid + 1 -- compensate for the rounding down
        end
    end
    return -1
end

function Array.BinarySearch(array,item,comparer)
    return DoBinarySearch(array, 1, #array, item,comparer)
end