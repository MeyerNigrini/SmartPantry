import { useEffect, useState } from "react";
import { Title, Stack, Text, Notification } from "@mantine/core";
import BarcodeScanner from "../../../components/BarcodeScanner";
import ProductForm from "../components/ProductForm";
import { fetchProductByBarcode } from "../services/openFoodFactsService";
import type { Product } from "../types/productTypes";

export default function ScanProductPage() {
  const [scannedValue, setScannedValue] = useState<string | null>(null);
  const [product, setProduct] = useState<Product>({
    name: "",
    quantity: "",
    brands: "",
    categories: "",
  });
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!scannedValue) return;

    const loadProduct = async () => {
    try {
    const data = await fetchProductByBarcode(scannedValue);
    setProduct(data);
    setNotFound(false);
    setError("");
    } catch (err: unknown) {
    setNotFound(true);
    setProduct({ name: "", quantity: "", brands: "", categories: "" });

    if (err instanceof Error) {
        setError(err.message); // message: "Product not found" or "Failed to fetch product"
    } else {
        setError("Something went wrong");
    }
    }
    };

    loadProduct();
  }, [scannedValue]);

  return (
    <Stack>
      <Title>Scan or Add Product</Title>

      <BarcodeScanner onScan={setScannedValue} />

      {notFound && (
        <Text color="red">
          Product not found. Please enter the details manually.
        </Text>
      )}

      {error && (
        <Notification color="red" withCloseButton onClose={() => setError("")}>
            {error}
        </Notification>
        )}

      <ProductForm product={product} onChange={setProduct} />
    </Stack>
  );
}
