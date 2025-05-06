import { useMutation, useQueryClient } from "@tanstack/react-query"
import { deleteBook } from "../services/booksServices"
import { toast } from "sonner";
import { AxiosError } from "axios";

export const useDeleteBook = (bookId: number) => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => deleteBook(bookId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("books")
            });
            toast.success(`Deleted book with ID ${bookId}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while deleting book with ID ${bookId}.`);
        }
    });

    return {
        deleteBook: mutation.mutateAsync
    }
}