import { useQuery } from "@tanstack/react-query";
import { fetchBooks } from "../services/booksServices";
import { PagedResponse, Book } from "../types/types";

export const useBooks = (pageNumber: number, pageSize: number) => {
    return useQuery<PagedResponse<Book>>({
        queryKey: ["books", pageNumber, pageSize],
        queryFn: () => fetchBooks(pageNumber, pageSize)
    });
}