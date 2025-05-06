import { Book, PagedResponse } from "../types/types";
import { axiosInstance } from "./axios";

export const fetchBooks = async (pageNumber: number, pageSize: number): Promise<PagedResponse<Book>> => {
    const response = await axiosInstance.get<PagedResponse<Book>>('/books', {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
};

export const fetchBook = async (bookId: number): Promise<Book> => {
    const response = await axiosInstance.get<Book>(`/books/${bookId}`);
    return response.data;
};

interface CreateBookRequest {
    title: string;
    author: string;
    description: string;
    categoryId: number | null;
    quantity: number;
}

export const createBook = async (request: CreateBookRequest): Promise<Book> => {
    const response = await axiosInstance.post<Book>("/books", request);
    return response.data;
}

interface UpdateBookRequest {
    id: number;
    title: string;
    author: string;
    description: string;
    categoryId: number | null;
    quantity: number;
}

export const updateBook = async (request: UpdateBookRequest): Promise<Book> => {
    const response = await axiosInstance.put<Book>(`/books/${request.id}`, request);
    return response.data;
}

export const deleteBook = async (bookId: number): Promise<void> => {
    await axiosInstance.delete(`/books/${bookId}`);
}