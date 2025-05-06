import { useQuery } from "@tanstack/react-query";
import { getCategories } from "../services/categoriesServices";

export const useCategories = (pageNumber: number = 1, pageSize: number = 10, searchQuery: string = "") => {
    const { data, isLoading, isError } = useQuery({
        queryKey: ["categories", pageNumber, pageSize, searchQuery],
        queryFn: () => getCategories(pageNumber, pageSize, searchQuery)
    });

    return { data, isLoading, isError };
}