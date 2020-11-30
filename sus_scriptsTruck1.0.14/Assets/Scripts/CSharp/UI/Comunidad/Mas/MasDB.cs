using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasDB
{
    private List<WriterBookList> writerBookList;

    public List<WriterBookList> WriterBookList()
    {
        if (writerBookList==null)
        {
            writerBookList = new List<WriterBookList>();
        }

        return writerBookList;
    }
}
