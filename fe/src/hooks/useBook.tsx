import { useQuery } from "@tanstack/react-query";
import { fetchBook } from "../services/booksServices";
import { Book } from "../types/types";

export const useBook = (bookId: number) => {
    return useQuery<Book>({
        queryKey: ["book", bookId],
        queryFn: () => fetchBook(bookId)
    });
}