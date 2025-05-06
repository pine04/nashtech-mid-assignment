import { useQuery } from "@tanstack/react-query";
import { getCategory } from "../services/categoriesServices";
import { Category } from "../types/types";

export const useCategory = (categoryId: number) => {
    return useQuery<Category>({
        queryKey: ["category", categoryId],
        queryFn: () => getCategory(categoryId)
    });
}