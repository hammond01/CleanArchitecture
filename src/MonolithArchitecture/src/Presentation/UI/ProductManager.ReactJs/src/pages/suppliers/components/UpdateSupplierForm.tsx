import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message, Form } from 'antd';
import React, { useEffect } from 'react';
import { supplierApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void; supplierId?: string }

const UpdateSupplierForm: React.FC<Props> = ({ visible, onCancel, onSuccess, supplierId }) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = Form.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (supplierId) {
        supplierApi.getById(String(supplierId)).then(res => { if (!mounted) return; if (res && res.statusCode === 200) form.setFieldsValue(res.data); else messageApi.error(res?.message || 'Failed load'); }).catch(() => messageApi.error('Failed load'));
      } else { form.resetFields(); }
    }
    return () => { mounted = false; };
  }, [visible, supplierId]);

  return (
    <>
      {contextHolder}
      <ModalForm form={form} title={<FormattedMessage id="pages.supplier.edit" defaultMessage="Edit Supplier" />} width="420px" visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try {
          let res: any = null;
          if (supplierId) res = await supplierApi.update(String(supplierId), vals);
          else res = await supplierApi.create(vals as any);
          if (res && (res.statusCode === 200 || res.statusCode === 201)) { message.success('Saved'); onSuccess(); return true; }
          message.error(res?.message || 'Failed to save'); return false;
        } catch { message.error('Failed to save'); return false; }
      }}>
        <ProFormText name="companyName" label="Company" rules={[{ required: true }]} />
        <ProFormText name="contactName" label="Contact" />
        <ProFormText name="phone" label="Phone" />
        <ProFormText name="country" label="Country" />
      </ModalForm>
    </>
  );
};

export default UpdateSupplierForm;
