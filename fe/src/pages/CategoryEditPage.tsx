import { useParams } from "react-router-dom"
import { useCategory } from "../hooks/useCategory";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorScreen } from "../components/ErrorScreen";
import { Category } from "../types/types";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "../components/ui/button";
import { Form, FormField, FormItem, FormLabel, FormControl, FormMessage } from "../components/ui/form";
import { Input } from "../components/ui/input";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateCategory } from "../services/categoriesServices";
import { toast } from "sonner";
import { AxiosError } from "axios";

export const CategoryEditPage = () => {
    const { id } = useParams();

    const { data: category, isLoading, isError } = useCategory(Number(id));

    return (
        <>
            {
                isLoading && <LoadingSpinner />
            }
            {
                isError && <ErrorScreen message="Cannot get category." />
            }
            {
                category &&
                <div className="p-8">
                    <div className="w-full max-w-7xl mx-auto">
                        <h1 className="mb-8 text-3xl font-bold">Edit category with ID {category.id}</h1>
                        <CategoryEditForm category={category} />
                    </div>
                </div>
            }
        </>
    )
}

interface CategoryEditFormProps {
    category: Category;
}

const formSchema = z.object({
    name: z.string().nonempty(),
});

const useUpdateCategory = () => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: updateCategory,
        onSuccess: (category: Category) => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("categories") || (query.queryKey.includes("category") && query.queryKey.includes(category.id))
            });
            toast.success(`Edited category with ID ${category.id}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while editing category.`);
        }
    });

    return {
        updateCategory: mutation.mutateAsync
    }
}

export const CategoryEditForm = ({ category }: CategoryEditFormProps) => {
    const { updateCategory } = useUpdateCategory();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: category.name
        },
    });

    async function onSubmit(values: z.infer<typeof formSchema>) {
        updateCategory({
            id: category.id,
            ...values
        });
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">

                <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Name</FormLabel>
                            <FormControl>
                                <Input {...field} />
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