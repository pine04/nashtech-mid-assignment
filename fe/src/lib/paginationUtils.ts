export const toValidPageNumber = (value: string | null): number => {
    if (value === null || !/^\d+$/.test(value)) return 1;
    const pageNumber = parseInt(value);
    if (pageNumber < 1) return 1;
    return pageNumber;
};

export const ALLOWED_PAGE_SIZES = [5, 10, 20];

export const toValidPageSize = (value: string | null): number => {
    if (value === null || !/^\d+$/.test(value)) return ALLOWED_PAGE_SIZES[1];

    const pageSize = parseInt(value);

    if (!ALLOWED_PAGE_SIZES.includes(pageSize)) return ALLOWED_PAGE_SIZES[1];

    return pageSize;
};
