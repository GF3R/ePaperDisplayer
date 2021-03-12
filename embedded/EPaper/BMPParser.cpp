//this is old code I did not want to delete due to its complexity..

static const uint16_t input_buffer_pixels = 640; // may affect performance

static const uint16_t max_palette_pixels = 256; // for depth <= 8

uint8_t input_buffer[3 * input_buffer_pixels];        // up to depth 24
uint8_t mono_palette_buffer[max_palette_pixels / 8];  // palette buffer for depth <= 8 b/w
uint8_t color_palette_buffer[max_palette_pixels / 8]; // palette buffer for depth <= 8 c/w

uint32_t skip(byte *byteArray, int32_t bytes)
{
    int32_t remain = bytes;
    uint32_t start = millis();
    while (remain > 0)
    {
        ++byteArray;
        remain--;
    }
    return bytes - remain;
}

void displayBitmap(byte *bytes)
{
    bool with_color = false;
    int16_t x = display.width() / 2;
    int16_t y = display.height() / 2;
    bool valid = false; // valid format to be handled
    bool flip = true;   // bitmap is stored bottom-to-top
    uint32_t startTime = millis();
    read16(bytes);
    bytes++;
    bytes++;
    uint32_t fileSize = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t creatorBytes = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t imageOffset = read32(bytes); // Start of image data
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t headerSize = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t width = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t height = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint16_t planes = read16(bytes);
    bytes++;
    bytes++;

    uint16_t depth = read16(bytes); // bits per pixel
    bytes++;
    bytes++;

    uint32_t format = read32(bytes);
    bytes++;
    bytes++;
    bytes++;
    bytes++;

    uint32_t bytes_read = 7 * 4 + 3 * 2; // read so far
    Serial.print("File size: ");
    Serial.println(fileSize);
    Serial.print("Image Offset: ");
    Serial.println(imageOffset);
    Serial.print("Header size: ");
    Serial.println(headerSize);
    Serial.print("Bit Depth: ");
    Serial.println(depth);
    Serial.print("Image size: ");
    Serial.print(width);
    Serial.print('x');
    Serial.println(height);
    Serial.print("format");
    Serial.println(format);
    Serial.print("planes");
    Serial.println(planes);

    if ((planes == 1) && ((format == 0) || (format == 3)) || true) // uncompressed is handled, 565 also
    {
        Serial.println("Working");
        Serial.print("File size: ");
        Serial.println(fileSize);
        Serial.print("Image Offset: ");
        Serial.println(imageOffset);
        Serial.print("Header size: ");
        Serial.println(headerSize);
        Serial.print("Bit Depth: ");
        Serial.println(depth);
        Serial.print("Image size: ");
        Serial.print(width);
        Serial.print('x');
        Serial.println(height);
        // BMP rows are padded (if needed) to 4-byte boundary
        uint32_t rowSize = (width * depth / 8 + 3) & ~3;
        if (depth < 8)
            rowSize = ((width * depth + 8 - depth) / 8 + 3) & ~3;
        if (height < 0)
        {
            height = -height;
            flip = false;
        }
        uint16_t w = width;
        uint16_t h = height;
        if ((x + w - 1) >= display.width())
            w = display.width() - x;
        if ((y + h - 1) >= display.height())
            h = display.height() - y;
        valid = true;
        uint8_t bitmask = 0xFF;
        uint8_t bitshift = 8 - depth;
        uint16_t red, green, blue;
        bool whitish, colored;
        if (depth == 1)
            with_color = false;
        if (depth <= 8)
        {
            if (depth < 8)
                bitmask >>= depth;

            int remain = imageOffset - (4 << depth) - bytes_read;
            while (remain > 0)
            {
                bytes++;
                bytes_read++;
                remain--;
            }
            for (uint16_t pn = 0; pn < (1 << depth); pn++)
            {
                ++bytes;
                blue = *bytes;
                ++bytes;
                green = *bytes;
                ++bytes;
                red = *bytes;
                ++bytes;
                bytes_read += 4;
                whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
                colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0));                                                  // reddish or yellowish?
                if (0 == pn % 8)
                    mono_palette_buffer[pn / 8] = 0;
                mono_palette_buffer[pn / 8] |= whitish << pn % 8;
                if (0 == pn % 8)
                    color_palette_buffer[pn / 8] = 0;
                color_palette_buffer[pn / 8] |= colored << pn % 8;
                //Serial.print("0x00"); Serial.print(red, HEX); Serial.print(green, HEX); Serial.print(blue, HEX);
                //Serial.print(" : "); Serial.print(whitish); Serial.print(", "); Serial.println(colored);
            }
        }
        display.fillScreen(GxEPD_WHITE);
        uint32_t rowPosition = flip ? imageOffset + (height - h) * rowSize : imageOffset;
        //Serial.print("skip "); Serial.println(rowPosition - bytes_read);
        int remain = rowPosition - bytes_read;
        while (remain > 0)
        {
            bytes++;
            bytes_read++;
            remain--;
        }
        for (uint16_t row = 0; row < h; row++, rowPosition += rowSize) // for each line
        {
            delay(1); // yield() to avoid WDT
            uint32_t in_remain = rowSize;
            uint32_t in_idx = 0;
            uint32_t in_bytes = 0;
            uint8_t in_byte = 0; // for depth <= 8
            uint8_t in_bits = 0; // for depth <= 8
            uint16_t color = GxEPD_WHITE;
            for (uint16_t col = 0; col < w; col++) // for each pixel
            {
                yield();

                // Time to read more pixel data?
                if (in_idx >= in_bytes) // ok, exact match for 24bit also (size IS multiple of 3)
                {
                    uint32_t get = in_remain > sizeof(input_buffer) ? sizeof(input_buffer) : in_remain;
                    uint8_t *buffer = input_buffer;
                    remain = get;
                    while ((remain > 0))
                    {
                        bytes++;
                        int16_t v = *bytes;
                        *buffer++ = uint8_t(v);
                        remain--;
                    }
                    uint32_t got = get - remain;
                    while (got < get)
                    {
                        buffer = input_buffer + got;
                        remain = get - got;
                        while ((remain > 0))
                        {
                            bytes++;
                            int16_t v = *bytes;
                            *buffer++ = uint8_t(v);
                            remain--;
                        }
                        uint32_t gotmore = get - got - remain;
                        got += gotmore;
                    }
                    in_bytes = got;
                    in_remain -= got;
                    bytes_read += got;
                }
                switch (depth)
                {
                case 24:
                    blue = input_buffer[in_idx++];
                    green = input_buffer[in_idx++];
                    red = input_buffer[in_idx++];
                    whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
                    colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0));                                                  // reddish or yellowish?
                    break;
                case 16:
                {
                    uint8_t lsb = input_buffer[in_idx++];
                    uint8_t msb = input_buffer[in_idx++];
                    if (format == 0) // 555
                    {
                        blue = (lsb & 0x1F) << 3;
                        green = ((msb & 0x03) << 6) | ((lsb & 0xE0) >> 2);
                        red = (msb & 0x7C) << 1;
                    }
                    else // 565
                    {
                        blue = (lsb & 0x1F) << 3;
                        green = ((msb & 0x07) << 5) | ((lsb & 0xE0) >> 3);
                        red = (msb & 0xF8);
                    }
                    whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
                    colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0));                                                  // reddish or yellowish?
                }
                break;
                case 1:
                case 4:
                case 8:
                {
                    if (0 == in_bits)
                    {
                        in_byte = input_buffer[in_idx++];
                        in_bits = 8;
                    }
                    uint16_t pn = (in_byte >> bitshift) & bitmask;
                    whitish = mono_palette_buffer[pn / 8] & (0x1 << pn % 8);
                    colored = color_palette_buffer[pn / 8] & (0x1 << pn % 8);
                    in_byte <<= depth;
                    in_bits -= depth;
                }
                break;
                }
                if (whitish)
                {
                    color = GxEPD_WHITE;
                }
                else
                {
                    color = GxEPD_BLACK;
                }
                uint16_t yrow = y + (flip ? h - row - 1 : row);
                display.drawPixel(x + col, yrow, color);
                /*
        Serial.print("drawing pixel x:");
        Serial.print(x + col);
        Serial.print(" y:");
        Serial.print(yrow);
        Serial.print("color");
        Serial.print(color);
        Serial.println();
        */
            } // end pixel
        }     // end line
    }
    Serial.print("bytes read ");
    Serial.println(bytes_read);
}

uint16_t read16(uint8_t *bytes)
{
    uint16_t result;
    ((uint8_t *)&result)[0] = *bytes; // LSB
    bytes++;
    ((uint8_t *)&result)[1] = *bytes; // MSB
    return result;
}

uint32_t read32(uint8_t *bytes)
{

    uint32_t result;
    ((uint8_t *)&result)[0] = *bytes; // LSB
    bytes++;
    ((uint8_t *)&result)[1] = *bytes;
    bytes++;
    ((uint8_t *)&result)[2] = *bytes;
    bytes++;
    ((uint8_t *)&result)[3] = *bytes; // MSB
    return result;
}
