/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.tom_roush.pdfbox.pdmodel.font;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;

import com.tom_roush.pdfbox.contentstream.PDContentStream;
import com.tom_roush.pdfbox.contentstream.operator.Operator;
import com.tom_roush.pdfbox.cos.COSBase;
import com.tom_roush.pdfbox.cos.COSNumber;
import com.tom_roush.pdfbox.cos.COSObject;
import com.tom_roush.pdfbox.cos.COSStream;
import com.tom_roush.pdfbox.pdfparser.PDFStreamParser;
import com.tom_roush.pdfbox.pdmodel.PDResources;
import com.tom_roush.pdfbox.pdmodel.common.COSObjectable;
import com.tom_roush.pdfbox.pdmodel.common.PDRectangle;
import com.tom_roush.pdfbox.pdmodel.common.PDStream;
import com.tom_roush.pdfbox.util.Matrix;

/**
 * A Type 3 character procedure. This is a standalone PDF content stream.
 *
 * @author John Hewson
 */
public final class PDType3CharProc implements COSObjectable, PDContentStream
{
    private final PDType3Font font;
    private final COSStream charStream;

    public PDType3CharProc(PDType3Font font, COSStream charStream)
    {
        this.font = font;
        this.charStream = charStream;
    }

    @Override
    public COSStream getCOSObject()
    {
        return charStream;
    }

    public PDType3Font getFont()
    {
        return font;
    }

    public PDStream getContentStream()
    {
        return new PDStream(charStream);
    }

    @Override
    public InputStream getContents() throws IOException
    {
        return charStream.createInputStream();
    }

    @Override
    public PDResources getResources()
    {
        return font.getResources();
    }

    @Override
    public PDRectangle getBBox()
    {
        return font.getFontBBox();
    }

    /**
     * Calculate the bounding box of this glyph. This will work only if the first operator in the
     * stream is d1.
     *
     * @return the bounding box of this glyph, or null if the first operator is not d1.
     * @throws IOException If an io error occurs while parsing the stream.
     */
    public PDRectangle getGlyphBBox() throws IOException
    {
        List<COSBase> arguments = new ArrayList<COSBase>();
        PDFStreamParser parser = new PDFStreamParser(this);
        Object token = parser.parseNextToken();
        while (token != null)
        {
            if (token instanceof COSObject)
            {
                arguments.add(((COSObject)token).getObject());
            }
            else if (token instanceof Operator)
            {
                if (((Operator)token).getName().equals("d1") && arguments.size() == 6)
                {
                    for (int i = 0; i < 6; ++i)
                    {
                        if (!(arguments.get(i) instanceof COSNumber))
                        {
                            return null;
                        }
                    }
                    return new PDRectangle(((COSNumber)arguments.get(2)).floatValue(),
                        ((COSNumber)arguments.get(3)).floatValue(), ((COSNumber)arguments.get(4))
                        .floatValue() - ((COSNumber)arguments.get(2)).floatValue(),
                        ((COSNumber)arguments.get(5)).floatValue() - ((COSNumber)arguments.get(3))
                            .floatValue());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                arguments.add((COSBase)token);
            }
            token = parser.parseNextToken();
        }
        return null;
    }

    @Override
    public Matrix getMatrix()
    {
        return font.getFontMatrix();
    }

    /**
     * todo.
     *
     * @return
     * @throws IOException
     */
    public float getWidth() throws IOException
    {
        List<COSBase> arguments = new ArrayList<COSBase>();
        PDFStreamParser parser = new PDFStreamParser(this);
        Object token = parser.parseNextToken();
        while (token != null)
        {
            if (token instanceof COSObject)
            {
                arguments.add(((COSObject)token).getObject());
            }
            else if (token instanceof Operator)
            {
                return parseWidth((Operator)token, arguments);
            }
            else
            {
                arguments.add((COSBase)token);
            }
            token = parser.parseNextToken();
        }
        throw new IOException("Unexpected end of stream");
    }

    private float parseWidth(Operator operator, List arguments) throws IOException
    {
        if (operator.getName().equals("d0") || operator.getName().equals("d1"))
        {
            Object obj = arguments.get(0);
            if (obj instanceof Number)
            {
                return ((Number)obj).floatValue();
            }
            else if (obj instanceof COSNumber)
            {
                return ((COSNumber)obj).floatValue();
            }
            else
            {
                throw new IOException("Unexpected argument type: " + obj.getClass().getName());
            }
        }
        else
        {
            throw new IOException("First operator must be d0 or d1");
        }
    }
}
