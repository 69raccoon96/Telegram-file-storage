import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";

export const Api = createApi({
    reducerPath: "filesApi",
    baseQuery: fetchBaseQuery({baseUrl: "http://localhost:5001/api"}),
    endpoints: (builder) => ({
        getChats: builder.query({
            query: () => ({
                url: `/chats`
            })
        })
    })
})

