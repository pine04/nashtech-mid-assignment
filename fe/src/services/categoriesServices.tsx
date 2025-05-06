import { Category, PagedResponse } from "../types/types";
import { axiosInstance } from "./axios";

export const getCategories = async (pageNumber: number = 1, pageSize: number = 10, searchQuery: string = ""): Promise<PagedResponse<Category>> => {
    const response = await axiosInstance.get<PagedResponse<Category>>("/categories", {
        params: {
            pageNumber,
            pageSize,
            searchQuery
        }
    });
    return response.data;
}

export const getCategory = async (categoryId: number): Promise<Category> => {
    const response = await axiosInstance.get<Category>(`/categories/${categoryId}`);
    return response.data;
};

interface CreateCategoryRequest {
    name: string;
}

export const createCategory = async (request: CreateCategoryRequest): Promise<Category> => {
    const response = await axiosInstance.post<Category>("/categories", request);
    return response.data;
}

interface UpdateCategoryRequest {
    id: number;
    name: string;
}

export const updateCategory = async (request: UpdateCategoryRequest): Promise<Category> => {
    const response = await axiosInstance.put<Category>(`/categories/${request.id}`, request);
    return response.data;
}

export const deleteCategory = async (categoryId: number): Promise<void> => {
    await axiosInstance.delete(`/categories/${categoryId}`);
}