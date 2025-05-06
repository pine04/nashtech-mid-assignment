import { useParams } from "react-router-dom"
import { useBook } from "../hooks/useBook";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorScreen } from "../components/ErrorScreen";
import { Book } from "../types/types";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "../components/ui/button";
import { Form, FormField, FormItem, FormLabel, FormControl, FormMessage } from "../components/ui/form";
import { Input } from "../components/ui/input";
import { Textarea } from "../components/ui/textarea";
import { CategorySelector } from "../components/CategorySelector";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateBook } from "../services/booksServices";
import { toast } from "sonner";
import { AxiosError } from "axios";

export const BookEditPage = () => {
    const { id } = useParams();

    const { data: book, isLoading, isError } = useBook(Number(id));

    return (
        <>
            {
                isLoading && <LoadingSpinner />
            }
            {
                isError && <ErrorScreen message="Cannot get book." />
            }
            {
                book &&
                <div className="p-8">
                    <div className="w-full max-w-7xl mx-auto">
                        <h1 className="mb-8 text-3xl font-bold">Edit book with ID {book.id}</h1>
                        <BookEditForm book={book} />
                    </div>
                </div>
            }
        </>
    )
}

interface BookEditFormProps {
    book: Book;
}

const formSchema = z.object({
    title: z.string().nonempty(),
    author: z.string().nonempty(),
    description: z.string(),
    categoryId: z.number().int().nullable(),
    quantity: z.number().int().gte(0),
});

const useUpdateBook = () => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: updateBook,
        onSuccess: (book: Book) => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("books") || (query.queryKey.includes("book") && query.queryKey.includes(book.id))
            });
            toast.success(`Edited book with ID ${book.id}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while editing book.`);
        }
    });

    return {
        updateBook: mutation.mutateAsync
    }
}

export const BookEditForm = ({ book }: BookEditFormProps) => {
    const { updateBook } = useUpdateBook();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            title: book.title,
            author: book.author,
            description: book.description,
            categoryId: book.categoryId || null,
            quantity: book.quantity,
        },
    });

    async function onSubmit(values: z.infer<typeof formSchema>) {
        updateBook({
            id: book.id,
            ...values
        });
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">

                <FormField
                    control={form.control}
                    name="title"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Title</FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="author"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Author</FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="description"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Description</FormLabel>
                            <FormControl>
                                <Textarea {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="categoryId"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Category</FormLabel>
                            <FormControl>
                                <CategorySelector initialSearchQuery={book.category || ""} value={field.value} setValue={(id) => form.setValue("categoryId", id)} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="quantity"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Quantity</FormLabel>
                            <FormControl>
                                <Input type="number" {...field} min={0} onChange={(e) => form.setValue("quantity", parseInt(e.target.value || "0", 10))} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <Button type="submit" className="bg-blue-800 w-fit self-center hover:bg-blue-600" disabled={false}>
                    Save changes
                </Button>
            </form>
        </Form>
    )
}