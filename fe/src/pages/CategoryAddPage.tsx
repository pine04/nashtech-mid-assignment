import { Category } from "../types/types";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "../components/ui/button";
import { Form, FormField, FormItem, FormLabel, FormControl, FormMessage } from "../components/ui/form";
import { Input } from "../components/ui/input";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createCategory } from "../services/categoriesServices";
import { toast } from "sonner";
import { AxiosError } from "axios";

export const CategoryAddPage = () => {
    return (
        <div className="p-8">
            <div className="w-full max-w-7xl mx-auto">
                <h1 className="mb-8 text-3xl font-bold">Add new category</h1>
                <CategoryAddForm />
            </div>
        </div>
    )
}

const formSchema = z.object({
    name: z.string().nonempty(),
});

const useAddCategories = () => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: createCategory,
        onSuccess: (category: Category) => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("categories")
            });
            toast.success(`Created new category with ID ${category.id}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while creating category.`);
        }
    });

    return {
        addCategory: mutation.mutateAsync
    }
}

export const CategoryAddForm = () => {
    const { addCategory } = useAddCategories();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: ""
        },
    });

    async function onSubmit(values: z.infer<typeof formSchema>) {
        addCategory(values);
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
                    Add category
                </Button>
            </form>
        </Form>
    )
}