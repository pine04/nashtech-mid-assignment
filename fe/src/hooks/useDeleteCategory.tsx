import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner";
import { AxiosError } from "axios";
import { deleteCategory } from "../services/categoriesServices";

export const useDeleteCategory = (categoryId: number) => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => deleteCategory(categoryId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("categories")
            });
            toast.success(`Deleted category with ID ${categoryId}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while deleting category with ID ${categoryId}.`);
        }
    });

    return {
        deleteCategory: mutation.mutateAsync
    }
}