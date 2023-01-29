package org.lexapech;

import com.google.gson.Gson;

import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.util.Objects;

public class JsonReader {

    public JsonReader() {

    }


    public Object readJson(String name) {
        try (Reader jsonReader = new InputStreamReader(Objects.requireNonNull(this.getClass().getResourceAsStream(name)))) {
            return new Gson().fromJson(jsonReader, Object.class);
        }
        catch (IOException | NullPointerException e) {
            e.printStackTrace();
        }
        return null;
    }

}
